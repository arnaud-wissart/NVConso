# NVConso

🎛️ Utilitaire Windows léger pour ajuster dynamiquement la limite de consommation électrique de votre carte graphique NVIDIA (Power Limit), directement depuis la zone de notification.

## 🚀 Fonctionnalités

- Icône discrète dans le tray Windows
- Deux modes d’alimentation :
  - 🧘 **Éco** (ex : 10% du TDP maximum)
  - 🔥 **Performance** (TDP maximal autorisé)
- Ajustement immédiat via NVML (API NVIDIA officielle)
- Démarrage rapide, silencieux, sans interface visible
- Compatible avec les applications bureautiques, de développement, ou de jeu

## 📷 Aperçu

![screenshot](./screenshot_nvconso.png)

## 🛠️ Prérequis

- .NET 8
- Drivers NVIDIA récents avec `nvml.dll` accessible
- Lancement en **mode administrateur** (requis par NVML)

## 📦 Installation

1. Téléchargez la dernière version depuis l’onglet [Releases](https://github.com/arnaud-wissart/NVConso/releases)
2. Lancez `NVConso.exe` en tant qu’administrateur
3. Cliquez sur l’icône dans la zone de notification pour choisir un mode

## ⚠️ Attention

- L'application ne fonctionne qu'avec des cartes NVIDIA compatibles NVML.
- Le réglage de la limite de consommation n'est pas instantanément visible dans l'app NVIDIA officielle, mais visible dans **GPU-Z**.

## 🧪 À venir (roadmap possible)

- Choix de valeurs personnalisées
- Mode automatique en fonction du type d’activité
- Ajout d’un profil "Silent", "Turbo", etc.

## 🔖 Mots-clés GitHub

#NVIDIA #GPU #PowerLimit #NVML #TrayApp #Performance #EcoMode #DevTools

---

© 2025 Arnaud Wissart
