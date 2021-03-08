using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Hasenpfeffer
{
    [Serializable]
    public class CardSelectedEvent : UnityEvent<Card>
    {
    }

    public class MouseActions : MonoBehaviour
    {
        public CardSelectedEvent OnCardSelected = new CardSelectedEvent();

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Card card = MouseOverCard();

                if (card != null)
                {
                    Debug.Log("CallCardSelected");
                    OnCardSelected.Invoke(card);
                }
            }
        }

        /*       private double clickdelay = 0.25f;

               public CardSelectedEvent OnCardSelected = new CardSelectedEvent();

               public void Start()
               {
                   StartCoroutine(ClickListener());
               }

               IEnumerator ClickListener()
               {
                   while (enabled)
                   {
                       if (Input.GetMouseButtonDown(0))
                       {
                           yield return MouseClick();
                       }

                       yield return null;
                   }
               }

               IEnumerator MouseClick()
               {
                   yield return new WaitForEndOfFrame();

                   float timeCount = 0;
                   while (timeCount < clickdelay)
                   {
                       if (Input.GetMouseButtonDown(0))
                       {
                           DoubleClick();
                           yield break;
                       }

                       timeCount += Time.deltaTime;
                       yield return null;
                   }

                   SingleClick();
               }

               public void DoubleClick()
               {
                   Debug.Log("Double Click: ");
               }

               private void SingleClick()
               {
                   Debug.Log("Single");
                   Card card = MouseOverCard();

                   if (card != null)
                   {
                       Debug.Log("CallCardSelected");
                       OnCardSelected.Invoke(card);
                   }
               }
               */

        /*       void Update()
               {

                   if (Input.GetMouseButtonUp(0))
                   {
                       clicked++;
                       if (clicked == 1)
                       {
                           clicktime = Time.time;
                           Debug.Log("clicked 1 " + clicktime);
                       }

                       if (clicked > 1 && Time.time - clicktime < clickdelay)
                       {
                           clicked = 0;
                           clicktime = 0;
                           Debug.Log("Double CLick: ");
                           Card card = MouseOverCard();

                           if (card != null)
                           {
                               Debug.Log("CallCardSelected");
                               OnCardSelected.Invoke(card);
                           }

                       }
                       else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
                   }
               }*/

        Card MouseOverCard()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit)
            {
                Card card = hit.collider.gameObject.GetComponent<Card>();
                if (card != null)
                {
                    return card;
                }
            }

            return null;
        }

    }
}
