using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System.Text.RegularExpressions;
using DG.Tweening;

namespace RobbieWagnerGames
{
    public partial class DialogueManager : MonoBehaviour
    {

        private Coroutine leftSpriteSwapCoroutine = null;
        private Coroutine rightSpriteSwapCoroutine = null;

        private void OnNextDialogueLine(InputAction.CallbackContext context)
        {
            if (CanContinue && !DialogueHasChoices())
            {
                continueIcon.enabled = false;

                StartCoroutine(ReadNextSentence());
            }
            else if(sentenceTyping)
            {
                skipSentenceTyping = true;
            }
        }

        public IEnumerator ReadNextSentence()
        {
            CanContinue = false;
            RemoveChoiceGameObjects();

            yield return null;
            if(currentStory.canContinue)
            {
                currentText.text = "";
                sentenceTyping = true;

                currentSentenceText = ConfigureSentence(currentStory.Continue());

                ConfigureTextField();
                char nextChar;
                for(int i = 0; i < currentSentenceText.Length; i++)
                {
                    if(skipSentenceTyping) break;
                    nextChar = currentSentenceText[i];
                    if(nextChar == '<')
                    {
                        while(nextChar != '>' && i < currentSentenceText.Length)
                        {
                            currentText.text += nextChar;
                            i++;
                            nextChar = currentSentenceText[i];
                        } 
                    }

                    currentText.text += nextChar;
                    yield return new WaitForSeconds(.036f);
                }

                currentText.text = currentSentenceText;
                sentenceTyping = false;
                skipSentenceTyping = false;
                CanContinue = true;
            }
            else
            {
                yield return StartCoroutine(EndDialogue());
            }
        }

        private void ConfigureTextField()
        {
            List<string> tags = currentStory.currentTags;

            string speaker = "";
            Sprite characterSprite = null;
            bool removeSpriteOnLeft = false;
            bool removeSpriteOnRight = false;
            bool? placeSpriteOnLeft = null;

            foreach(string t in tags)
            {
                string tag = Regex.Replace(t, @"\s+", "");
                tag = tag.Replace("~"," ");
                tag = tag.ToUpper();

                #region tag conditions
                if (tag.Contains("SPEAKERISONLEFT"))
                {
                    currentSpeakerIsOnLeft = true;
                }
                else if(tag.Contains("SPEAKERISONRIGHT"))
                {
                    currentSpeakerIsOnLeft = false;
                }

                else if(tag.Contains("PLACESPRITEONLEFT"))
                {
                    tag = tag.Remove(tag.IndexOf("PLACESPRITEONLEFT"), 17).ToLower();
                    string filePath = StaticGameStats.characterSpriteFilePath + tag;
                    characterSprite = Resources.Load<Sprite>(filePath);
                    if(characterSprite == null) Debug.LogWarning($"sprite name provided \"{tag}\" is not a valid sprite name, please reconfigure this tag");
                    placeSpriteOnLeft = true;
                }
                else if(tag.Contains("PLACESPRITEONRIGHT"))
                {
                    tag = tag.Remove(tag.IndexOf("PLACESPRITEONRIGHT"), 18).ToLower();
                    string filePath = StaticGameStats.characterSpriteFilePath + tag;
                    characterSprite = Resources.Load<Sprite>(filePath);
                    if(characterSprite == null) Debug.LogWarning($"sprite name provided \"{tag}\" is not a valid sprite name, please reconfigure this tag");
                    placeSpriteOnLeft = false;
                }

                else if(tag.Contains("REMOVESPRITEONLEFT"))
                {
                    removeSpriteOnLeft = true;
                }
                else if(tag.Contains("REMOVESPRITEONRIGHT"))
                {
                    removeSpriteOnRight = true;
                }
                else if(tag.Contains("REMOVESPRITES"))
                {
                    removeSpriteOnLeft = true;
                    removeSpriteOnRight = true;
                }

                else if(tag.Contains("LEFTSHAKESPRITE"))
                {
                    StartCoroutine(ShakeSprite(leftSpeakerSprite, 40f));
                }
                else if(tag.Contains("RIGHTSHAKESPRITE"))
                {
                    StartCoroutine(ShakeSprite(rightSpeakerSprite, 40f));
                }

                else if(tag.Contains("SHAKEBACKGROUND"))
                {
                    StartCoroutine(ShakeSprite(backgroundImage, 15f));
                }
                else if(tag.Contains("SHOWBACKGROUND"))
                {
                    backgroundImage.color = Color.clear;
                    backgroundImage.enabled = true;
                    backgroundImage.DOColor(Color.white, .75f);
                }
                else if(tag.Contains("HIDEBACKGROUND"))
                {
                    backgroundImage.DOColor(Color.clear, .75f);
                    backgroundImage.enabled = false;
                }
                else if(tag.Contains("CHANGEBACKGROUNDIMMEDIATE"))
                {
                    tag = tag.Remove(tag.IndexOf("CHANGEBACKGROUND"), 16).ToLower();
                    string filePath = StaticGameStats.backgroundSpriteFilePath + tag;
                    Sprite sprite = Resources.Load<Sprite>(filePath);
                    if (sprite == null) 
                        Debug.LogWarning($"sprite name provided \"{tag}\" is not a valid sprite name, please reconfigure this tag");
                    else
                    {
                        backgroundImage.sprite = sprite;
                        backgroundImage.enabled = true;
                    }
                }
                else if(tag.Contains("CHANGEBACKGROUND"))
                {
                    tag = tag.Remove(tag.IndexOf("CHANGEBACKGROUND"), 16).ToLower();
                    string filePath = StaticGameStats.backgroundSpriteFilePath + tag;
                    Sprite sprite = Resources.Load<Sprite>(filePath);
                    if(sprite == null) 
                        Debug.LogWarning($"sprite name provided \"{tag}\" is not a valid sprite name, please reconfigure this tag");
                    StartCoroutine(ChangeBackground(sprite));
                }

                else if(tag.Contains("PLAYSOUND"))
                {
                    tag = tag.Remove(tag.IndexOf("PLAYSOUND"), 9).ToLower();
                    string filePath = StaticGameStats.soundFilePath + tag;
                    AudioClip soundClip = Resources.Load<AudioClip>(filePath);
                    if(soundClip == null) 
                        Debug.LogWarning($"sound name provided \"{tag}\" is not a sound sprite name, please reconfigure this tag");
                    else
                    {
                        dialogueSound.clip = soundClip;
                        dialogueSound.Play();
                    }
                }

                else if(tag.Contains("SPEAKER"))
                {
                    tag = tag.Remove(tag.IndexOf("SPEAKER"), 7);
                    speaker = tag.ToLower();
                }
                else
                    Debug.LogWarning($"\"{t}\" could not be read, please check its formatting/spelling it or have it removed. Refer to the docs for more information on proper tag writing");
                #endregion
            }

            if (!string.IsNullOrEmpty(speaker))
            {
                SetSpeaker(speaker, currentSpeakerIsOnLeft);
                ToggleSpeaker(leftSpeakerNamePlate, leftSpeakerName, currentSpeakerIsOnLeft);
                ToggleSpeaker(rightSpeakerNamePlate, rightSpeakerName, !currentSpeakerIsOnLeft);
            }
            else
            {
                ToggleSpeaker(leftSpeakerNamePlate, leftSpeakerName, false);
                ToggleSpeaker(rightSpeakerNamePlate, rightSpeakerName, false);
            }

            if(removeSpriteOnLeft) 
                leftSpriteSwapCoroutine = StartCoroutine(ToggleSprite(leftSpeakerSprite,  false, leftSpriteSwapCoroutine, characterSprite));
            if(removeSpriteOnRight) 
                rightSpriteSwapCoroutine = StartCoroutine(ToggleSprite(rightSpeakerSprite, false, rightSpriteSwapCoroutine, characterSprite));

            if(placeSpriteOnLeft == true)
                leftSpriteSwapCoroutine = StartCoroutine(ToggleSprite(leftSpeakerSprite, true, leftSpriteSwapCoroutine, characterSprite));
            else if(placeSpriteOnLeft == false)
                rightSpriteSwapCoroutine = StartCoroutine(ToggleSprite(rightSpeakerSprite, true, rightSpriteSwapCoroutine, characterSprite));
        }

        //TODO: include sprite placement
        private void SetSpeaker(string speaker, bool currentSpeakerIsOnLeft)
        {
            if(currentSpeakerIsOnLeft)
            {
                ToggleSpeaker(leftSpeakerNamePlate, leftSpeakerName, false);
                ToggleSpeaker(rightSpeakerNamePlate, rightSpeakerName, true);
                speaker = char.ToUpper(speaker[0]) + speaker.Substring(1);
                leftSpeakerName.text = speaker;
            }
            else
            {
                ToggleSpeaker(rightSpeakerNamePlate, rightSpeakerName, false);
                ToggleSpeaker(leftSpeakerNamePlate, leftSpeakerName, true);
                speaker = char.ToUpper(speaker[0]) + speaker.Substring(1);
                rightSpeakerName.text = speaker;
            }
        }
    }
}