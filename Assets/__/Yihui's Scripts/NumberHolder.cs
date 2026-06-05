using System;
using TMPro;
using UnityEngine;

// Readme: How to use: 挂在TextMeshProUGUI就行了, 用set_n就行了, enable就会开始操作
public enum NumberType
{
    Comma,
    Short,
    Long,
}
public class NumberHolder : MonoBehaviour
{
    #region EXPOSED
    public void init(double i_number = 0, NumberType type = NumberType.Comma, double speed = 1f, double init_x = 0.05f)
    {
        _type = type;
        _speed = Math.Clamp(speed, 1, 100);
        _init_x = Math.Clamp(init_x, 0, 0.2f);
        number = i_number;
    }

    public void add(double change)
    {
        if (change == 0) return;
        number += change;
    }

    public void set_n(double n)
    {
        add(n - number);
    }
    #endregion

    #region HIDEN
    [SerializeField]
    NumberType _type = NumberType.Comma;
    [SerializeField]
    double _speed = 1;
    [SerializeField]
    double _init_x = 0.05;
    public double _number;
    double _number_surface;
    TMP_Text text // Enable "Pixel Perfect" in Component "Canvas" 
    {
        get
        {
             return GetComponent<TMP_Text>();
        }
    }
    double number
    {
        get
        {
            return _number;
        }
        set
        {
            _number = value;
            if (!_is_init) _is_init = true;
            StartShowing();
        }
    }
    double number_surface
    {
        get
        {
            return _number_surface;
        }
        set
        {
            _number_surface = value;
            string result = "";
            switch(_type)
            {
                case NumberType.Comma:
                    result = CommaInt(number_surface);
                    break;
                case NumberType.Short:
                    result = ShortInt(number_surface);
                    break;
                case NumberType.Long:
                    result = Math.Floor(number_surface).ToString();
                    break;
            }
            text.text = result;
        }
    }
    // double _x_end_time = 10;
    void Update()
    {
        Show();
    }
    bool _is_init;
    void Start()
    {
        if (!_is_init)
            number = number;
    }
    double _start_n;
    double _end_n
    {
        get
        {
            return number - _start_n;
        }
    }
    double _x;
    // void Show()
    // {
    //     number_surface = _start_n + Math.Floor(    (1 - Math.Exp(- _x * _speed)) * _end_n        );
    //     _x += Time.deltaTime;
    //     if (_x >= _x_end_time) number_surface = number;
    //     if (number_surface == number) StopShowing();
    // }
    void Show()
    {
        double t = 1 - Math.Exp(-_x * _speed);
        if (number < 50) t = 1;

        // 很接近终点了，就直接显示最终数字
        if (t >= 0.995)
        {
            number_surface = number;
            StopShowing();
            return;
        }

        number_surface = _start_n + Math.Floor(t * _end_n);

        _x += Time.deltaTime;
    }
    void StartShowing()
    {
        enabled = true;
    }
    void OnEnable()
    {
        _x = _init_x;
        _start_n = number_surface;
    }
    void StopShowing()
    {
        enabled = false;
    }
    public static string CommaInt(double n)
    {
        return Math.Floor(n).ToString("N0");
    }

    public static string ShortInt(double n)
    {
        double floored = Math.Floor(n);
        double absN = Math.Abs(floored);

        if (absN < 1000.0)
            return n.ToString("N0");

        string[] units = { "K", "M", "B", "T", "Q" };
        double value = absN;
        int unitIndex = -1;

        while (value >= 1000.0 && unitIndex < units.Length - 1)
        {
            value /= 1000.0;
            unitIndex++;
        }

        string result = value.ToString("N0") + " " + units[unitIndex];
        return n < 0 ? "-" + result : result;
    }
    #endregion
}