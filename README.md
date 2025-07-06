# NVConso

🎛️ **NVConso** est un utilitaire Windows léger pour ajuster dynamiquement la **limite de consommation électrique (Power Limit)** de ta carte graphique **NVIDIA**, directement depuis la zone de notification Windows.

## 🚀 Fonctionnalités

- Icône discrète dans le **tray Windows**
- Deux modes d’alimentation :
  - 🧘 **Éco** : limite à ~10% du TDP max
  - 🔥 **Performance** : limite maximale autorisée
- Contrôle direct via **NVML** (API officielle NVIDIA)
- Démarrage rapide et silencieux (pas de fenêtre visible)
- Idéal pour :
  - Travailler (dev, bureautique) sans gaspillage énergétique
  - Passer en mode jeu d’un clic

## ✅ Tests

Ce projet inclut un projet de **tests unitaires** `NVConso.Tests`, basé sur **xUnit**, avec un **Mock de la couche NVML** permettant de tester sans carte NVIDIA réelle.

### 💻 Lancer les tests

```bash
dotnet test
```

## 🛠️ Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Carte graphique NVIDIA **compatible NVML**
- Fichier `nvml.dll` disponible dans le PATH ou à côté de l’exécutable
- Application **lancée en mode administrateur**

## 📦 Installation

1. Télécharge la dernière version depuis l’onglet [Releases](https://github.com/arnaud-wissart/NVConso/releases)
2. Lance `NVConso.exe` en **tant qu'administrateur**
3. Clique sur l’icône dans la zone de notification pour choisir un mode

## ⚠️ Remarques importantes

- Les modifications de Power Limit peuvent **ne pas s’afficher dans l'application NVIDIA officielle**.
- Pour une lecture fiable des valeurs, utilise un outil comme **GPU-Z**.
- Certaines limitations peuvent s’appliquer selon ta carte (notamment sur les portables).

## 🧭 Roadmap envisagée

- Valeurs personnalisées de limite (par pas de 5%)
- Profils "Turbo", "Silent", "Work", etc.
- Mode automatique basé sur l'activité ou l'utilisation CPU/GPU
- Démarrage automatique avec Windows
- Prise en charge multi-GPU

## 📄 Licence

Ce projet est sous licence **MIT** — libre d'utilisation, modification et redistribution.

## 🏷️ Tags GitHub

#NVIDIA #GPU #PowerLimit #NVML #TrayApp #Performance #EcoMode #WinForms #DotNet8 #DevTools

---

© 2025 Arnaud Wissart
