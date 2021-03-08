
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Threading.Tasks;
using System.Collections;

namespace Hasenpfeffer
{

    /// <summary>
    /// SetFaceUp(false) clears card's face value
    /// To display a card's value, call SetCardValue(byte) to assign the Rank and the Suit to the card, then call SetFaceUp(true)
    /// </summary>
    public class Card : MonoBehaviour
    {
        public GameDataManager gameDataManager;

        public static Ranks GetRank(byte value)
        {
            return (Ranks)(value % 12 / 2 + 1);
        }

        public static Suits GetSuit(byte value)
        {
            return (Suits)(value / 12);
        }

        public byte byteValue;

        public SpriteAtlas Atlas;

        public Suits Suit = Suits.NoTrump;
        public Ranks Rank = Ranks.NoRanks;

        public string OwnerId;

        SpriteRenderer spriteRenderer;

        bool faceUp = false;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        }

        private void Start()
        {
            UpdateSprite();
        }

        public void SetFaceUp(bool value)
        {
            faceUp = value;
            UpdateSprite();

            // Setting faceup to false also resets card's value.
            if (value == false)
            {
                Rank = Ranks.NoRanks;
                Suit = Suits.NoTrump;
            }
        }

        public void SetByteValue(byte value)
        {
            byteValue = value;
        }

        public void SetCardValue(byte value)
        {
            Rank = (Ranks)(value % 12 / 2 + 1);

            Suit = (Suits)(value / 12);
        }

        void UpdateSprite()
        {
            if (faceUp)
            {
                spriteRenderer.sprite = Atlas.GetSprite(SpriteName());
            }
            else
            {
                spriteRenderer.sprite = Atlas.GetSprite(Constants.CARD_BACK_SPRITE);
            }
        }

        public void DimSprite()
        {
            spriteRenderer.material.SetColor("_Color", Color.gray);
        }

        public bool SpriteColor(string checkColor)
        {
            bool color = spriteRenderer.material.color.ToString() == checkColor;
            Debug.Log(checkColor);
            Debug.Log(spriteRenderer.material.color.ToString());
            return color;
        }

        string GetRankDescription()
        {
            FieldInfo fieldInfo = Rank.GetType().GetField(Rank.ToString());
            DescriptionAttribute[] attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes[0].Description;
        }

        string SpriteName()
        {
            string testName = $"card{Suit}{GetRankDescription()}";
            return testName;
        }

        public void SetDisplayingOrder(int order)
        {
            spriteRenderer.sortingOrder = order;
        }

        public void OnSelected(bool selected)
        {
            if (selected)
            {
                transform.position = new Vector3(transform.position.x, -5.5f, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, -7, transform.position.z);
            }
        }

        private void OnMouseEnter()
        {
            if (transform.position.y == -7 || transform.position.y == -5.5)
            {
                transform.position = (Vector3)transform.position + Vector3.up * Constants.CARD_SELECTED_OFFSET;
                transform.localScale += new Vector3(1F, 1f, 1f);
            }
        }

        private void OnMouseExit()
        {
            if (transform.position.y == -6)
            {
                transform.position = (Vector3)transform.position - Vector3.up * Constants.CARD_SELECTED_OFFSET;
                transform.position = new Vector3(transform.position.x, -7, transform.position.z);
            }
            else if (transform.position.y == -4.5)
            {
                transform.position = (Vector3)transform.position - Vector3.up * Constants.CARD_SELECTED_OFFSET;
                transform.position = new Vector3(transform.position.x, -5.5f, transform.position.z);
            }
            transform.localScale = new Vector3(2.5F, 2.5F, 2.5f);
        }
    }
}


