using UnityEngine;
using TMPro;
using System.Text;
using System;

public class WaveView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentWave;

    public void UpdateWaveVisual(int wave)
    {
        _currentWave.text = ToRoman(wave);
    }

    private string ToRoman(int number)
    {
        if (number < 1 || number > 3999)
            throw new ArgumentOutOfRangeException("number", "Value must be between 1 and 3999.");

        var romanNumerals = new (int value, string numeral)[]
        {
        (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
        (100, "C"), (90, "XC"), (50, "L"), (40, "XL"),
        (10, "X"), (9, "IX"), (5, "V"), (4, "IV"),
        (1, "I")
        };

        var result = new StringBuilder();

        foreach (var (value, numeral) in romanNumerals)
        {
            while (number >= value)
            {
                result.Append(numeral);
                number -= value;
            }
        }

        return result.ToString();
    }
}
