using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Hasenpfeffer
{
    public class CardAnimation
    {
        Card card;
        Vector3 destination;
        Quaternion rotation;
 
        public CardAnimation(Card c, Vector3 pos)
        {
            card = c;
            destination = pos;
            rotation = Quaternion.identity;
        }

        public CardAnimation(Card c, Vector3 pos, Quaternion rot)
        {
            card = c;
            destination = pos;
            rotation = rot;
        }

        public bool Play()
        {
            bool finished = false;

            if (Vector3.Distance(card.transform.position, destination) < Constants.CARD_SNAP_DISTANCE)
            {
                card.transform.position = destination;
                finished = true;
            }
            else
            {
                card.transform.position = Vector3.MoveTowards(card.transform.position, destination, Constants.CARD_MOVEMENT_SPEED * Time.deltaTime);
                card.transform.rotation = Quaternion.Lerp(card.transform.rotation, rotation, Constants.CARD_ROTATION_SPEED * Time.deltaTime);
            }

            return finished;
        }
    }

    /// <summary>
    /// Controls all card animations in the game
    /// </summary>
    public class CardAnimator : MonoBehaviour
    {
        public GameObject CardPrefab;

        public List<Card> DisplayingCards;

        Queue<CardAnimation> cardAnimations;

        CardAnimation currentCardAnimation;

        Vector3 startPosition = new Vector3(0f, 1f);

        // invoked when all queued card animations have been played
        public UnityEvent OnAllAnimationsFinished = new UnityEvent();

        bool working = false;

        protected GameDataManager gameDataManager;

        void Awake()
        {
            cardAnimations = new Queue<CardAnimation>();
            InitializeDeck();
        }

        void InitializeDeck()
        {
            DisplayingCards = new List<Card>();

            for (byte value = 0; value < 48; value++)
            {
                Vector3 newPosition = startPosition + Vector3.right * Constants.DECK_CARD_POSITION_OFFSET * value;
                GameObject newGameObject = Instantiate(CardPrefab, newPosition, Quaternion.identity);
                newGameObject.transform.parent = transform;
                Card card = newGameObject.GetComponent<Card>();
                card.SetDisplayingOrder(-1);
                card.transform.position = newPosition;
                DisplayingCards.Add(card);
            }
        }

        public Card TakeFirstDisplayingCard()
        {
            int numberOfDisplayingCard = DisplayingCards.Count;

            if (numberOfDisplayingCard > 0)
            {
                Card card = DisplayingCards[numberOfDisplayingCard - 1];
                DisplayingCards.Remove(card);

                return card;
            }

            return null;
        }

        public IEnumerator DealDisplayingCards(Player player, int numberOfCard, bool animated)
        {
            if (DisplayingCards.Count == 0)
            {
                InitializeDeck();
            }

            int start = DisplayingCards.Count - 1;
            int finish = DisplayingCards.Count - 1 - numberOfCard;

            List<Card> cardsToRemoveFromDeck = new List<Card>();

            for (int i = start; i > finish; i--)
            {
                Card card = DisplayingCards[i];
                player.ReceiveDisplayingCard(card);
                cardsToRemoveFromDeck.Add(card);
                if (animated)
                {
                    AddCardAnimation(card, player.NextCardPosition(player, numberOfCard));
                }
                else
                {
                    card.transform.position = player.NextCardPosition(player, numberOfCard);
                }
            }

            foreach (Card card in cardsToRemoveFromDeck)
            {
                DisplayingCards.Remove(card);
            }

            yield return null;
        }

        public void RevealPlayedCard(byte playedCard, Player player)
        {
            GameObject newGameObject = Instantiate(CardPrefab, player.PlayPosition, Quaternion.identity);
            newGameObject.transform.parent = transform;
            Card card = newGameObject.GetComponent<Card>();
            card.SetCardValue(playedCard);
            card.SetFaceUp(true);
        }

        public void DrawDisplayingCard(Player player)
        {
            int numberOfDisplayingCard = DisplayingCards.Count;

            if (numberOfDisplayingCard > 0)
            {
                Card card = DisplayingCards[numberOfDisplayingCard - 1];
                player.ReceiveDisplayingCard(card);
                AddCardAnimation(card, player.NextCardPosition(player, numberOfDisplayingCard));

                DisplayingCards.Remove(card);
            }
        }

        public void DrawDisplayingCard(Player player, byte value)
        {
            int numberOfDisplayingCard = DisplayingCards.Count;

            if (numberOfDisplayingCard > 0)
            {
                Card card = DisplayingCards[numberOfDisplayingCard - 1];
                card.SetCardValue(value);
                card.SetFaceUp(true);
                player.ReceiveDisplayingCard(card);
                AddCardAnimation(card, player.NextCardPosition(player, numberOfDisplayingCard));

                DisplayingCards.Remove(card);
            }
        }

        public void AddCardAnimation(Card card, Vector3 position)
        {
            CardAnimation ca = new CardAnimation(card, position);
            cardAnimations.Enqueue(ca);
            working = true;
        }

        public void AddCardAnimation(Card card, Vector3 position, Quaternion rotation)
        {
            CardAnimation ca = new CardAnimation(card, position, rotation);
            cardAnimations.Enqueue(ca);
            working = true;
        }

        private void Update()
        {
            if (currentCardAnimation == null)
            {
                NextAnimation();
            }
            else
            {
                if (currentCardAnimation.Play())
                {
                    NextAnimation();
                }
            }
        }

        void NextAnimation()
        {
            currentCardAnimation = null;

            if (cardAnimations.Count > 0)
            {
                CardAnimation ca = cardAnimations.Dequeue();
                currentCardAnimation = ca;
            }
            else
            {
                if (working)
                {
                    working = false;
                    //OnAllAnimationsFinished.Invoke();
                }
            }
        }
    }
}
