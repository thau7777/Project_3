using UnityEngine;

namespace MyRule
{
    public class SuggestView : Singleton<SuggestView>
    {
        [SerializeField] private GameObject[] suggests;

        public void ShowSuggest(int index)
        {
            for (int i = 0; i < suggests.Length; i++)
            {
                if (i ==  index) suggests[i].SetActive(true);
                else suggests[i].SetActive(false);
            }
        }

        public void HideSuggest(int index)
        {
            suggests[index].SetActive(false);
        }    
    }
}