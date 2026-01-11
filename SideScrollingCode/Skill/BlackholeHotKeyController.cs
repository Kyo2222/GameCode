using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlackholeHotKeyController : MonoBehaviour
{
    private SpriteRenderer _spriteRender;
    private KeyCode _hotKey;
    private TextMeshProUGUI _text;
    private Transform _enemy;
    private BlackholeSkillController _blackhole;

    public void SetupHotKey(KeyCode hotKey, Transform enemy, BlackholeSkillController blackhole)
    {
        _spriteRender = GetComponent<SpriteRenderer>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _text.color = Color.white;
        _spriteRender.color = Color.black;
        _hotKey = hotKey;
        _text.text = _hotKey.ToString();

        _enemy = enemy;
        _blackhole = blackhole;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_hotKey))
        {
            _blackhole.AddTargetList(_enemy);
            _text.color = Color.clear;
            _spriteRender.color = Color.clear;
        }
    }
}
