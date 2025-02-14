using System.Collections;
using UnityEngine;
using TMPro;

public class LootView : MonoBehaviour
{
    [SerializeField] private TMP_Text _lootText;
    private Coroutine _typingCoroutine;

    public void UpdateLootText(string lootType)
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _typingCoroutine = StartCoroutine(TypeText($"Looted: {lootType}"));
    }

    private IEnumerator TypeText(string text)
    {
        _lootText.text = "";
        foreach (char letter in text)
        {
            _lootText.text += letter;
            yield return new WaitForSeconds(0.05f); 
        }

        yield return new WaitForSeconds(1f); // Give player time to read
        this.gameObject.SetActive(false);
    }
}
