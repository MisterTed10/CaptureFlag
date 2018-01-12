using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pawn : MonoBehaviour {

    private Core.Character m_character;
    public void setCharacter(Core.Character character)
    {
        m_character = character;
    }
	// Use this for initialization
	void Start () {
	}
	// Update is called once per frame
	void Update () 
    {
        if (m_character != null)
            m_character.Step(Time.deltaTime);
	}
}
