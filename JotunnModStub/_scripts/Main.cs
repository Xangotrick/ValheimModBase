using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;


internal class Main
{
    /// Lancé au Awake du mod
    public static void Awake()
    {
    }

    /// Lancé chaque update du mod
    public static void Update()
    {
    }

    /// Lancé chauqe udpate du mod fréquence fixe (Time.deltatime constant)
    public static void FixedUpdate()
    {
    }

    /// Lancé chaque seconde
    public static void Update1000()
    {
    }

    /// Lancé à la syncronisation du serveur, si option réseau utilisée
    public static void OnServerSync()
    {
    }

    /// Lancé au lancement du réseau/une fois que le monde et le personnage sont existant
    public static void OnGameStart()
    {
    }

    /// Lancé chaque update du mod une fois que le monde est chargé
    public static void GameUpdate()
    {
    }

    /// Lancé chaque seconde du jeu une fois que le monde est chargé
    public static void GameUpdate1000()
    {
    }

    /// Lancé une fois que le monde est déchargé
    public static void OnGameEnd()
    {
    }
}
