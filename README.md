<h1 align="center">VOICE RECOGNITION QUIZ GAME</h1>

## Abstract
The point of this project was to create Quiz game in Unity3D with use of cloud services and implementation
of vocal controlling.

Player interacts with game using mouse and microphone.

Player is able to see actual score, questions and communicate with the game using Google Speech-to-text API.

## Zadanie

*Kvízová hra. Vytvorenie systému odpovedania na otázky pomocou hlasu a získanie spätnej väzby pomocou Microsoft Face API. Reakcia na správnu/nespávnu odpoved sa mení na základe veku, pohlavia a emočného stavu hráča.

<p align="center">
  <img src="https://i.imgur.com/AyRLC9e.png">
</p>

## Tvorcovia
Na projekte sa podielali dvaja študenti Technickej univerzity v Košiciach. Každý z nich mal za úlohu implementovať niekoľko riešení, ktoré zabezpečujú dané funkcie projektu.

- ***Erik Klembara*** - Implementácia cloudovej služby Face API z prostredia Microsoft Azure, implementácia Speech-To-Text API služby z prostredia Google Cloud Platform.
- ***Oliver Tomko*** - Vytvorenie Unity3D projektu, vytvorenie kvízového systému a spracovanie odpovedí v hre.

## Architektúra projektu

### Unity projekt ###
- ***Assets***
  - **Animations** - *Obsahuje súbory pre základne animácie objektov*
    - **ResolutionScreen** - *Obsahuje animácie vyhonotenia odpovede (Correct/Wrong)*
    - **Timer** - *Obsahuje animácie časovača*
  - **Font** - *Obasahuje fonty použité v grafickom rozhraní*
  - **Prefabs** - *Obsahuje objekty použité v hre*
    - **Audio** - *Obsahuje objekt, ktorý spravuje spúšťanie muziky a zvukov hry*
    - **Game** - *Obsahuje objekt, ktorý spravuje hern0 udalosti*
    - **UI** - *Obsahuje objekt, ktorý zobrazuje odpovede otázok*
  - **Resources** - *Obsahuje externé zdroje pre hru*
    - **Questions** - *Obsahuje objekty, ktoré predstavujú jednotlivé otázky
  - **Scenes** - *Obsahuje scény, ktoré sa využívajú v projekte*
  - **Scripts** - *Obsahuje všetky scripty, ktoré zabezpečujú funkcie v projekte*
    - **Editor** - *Obsahuje obslužné skripty na výtvaranie otázok a odpovedí*
    - **Mono** - *Obsahuje obslužné skripty UI, spracovanie odpovedí hráča*
    - **ScriptableObject** - *Obsahuje classy otázky a herných udalostí*
    - **SavWav** - *Ukladá nahranú hovorenú odpoved hráčom vo formáte wav na lokálny disk*
    - **voice** - *Nahráva hlas hráča a následne odosiela zvukovú nahrávku vo forme requestu Google Speech-To-Text API*
    - **webcam** - *Výtvára snímok hráča, ktorý nasledne zasiela na Face API, ktorá ako odozvu vracia údaje vo forme json. ktoré obsahujú informácie o emóciach, veku a pohlaví hráča*

## Využité cloudové služby ##

### Face Recognition Api ###
- (Microsoft Azure, subskripcia Azure for Students, 30000 volaní mesačne zdarma), endpoint https://faceappcloudy.cognitiveservices.azure.com/
Hlavný skript sa nachádza v priečinku Assets/Scripts/displayWebcam.cs
Služba funguje ako RESTapi, kde vstupom je fotografia a výstupom je JSON obsahujúci výstup zo služby.
Pre správne fungovanie je nutné na disku vytvoriť priečinok C:\WebcamSnaps\, alebo nastaviť inú cestu pre ukladanie snímok z kamery.
Verifikáciu zabezpečuje porovnanie fotografie z webkamery s vytvorenou skupinou na Azure FaceAPI, kde má každý užívateľ pridelený jednoznačný identifikátor a vracia confidence level, ako veľmi sa tvár podobá s tvárou v databáze. Pre úspešnú verifikáciu musí mať daný užívateľ confidence aspoň 0.8.
Vzor výstupného JSONu: 
```C#
[{"faceId":"7a60c6b4-31a4-409c-86eb-8a59b75f5255","faceRectangle":{"top":202,"left":187,"width":203,"height":203},"faceAttributes":{"smile":0.0,"headPose":{"pitch":-8.1,"roll":2.2,"yaw":-0.6},"gender":"male","age":25.0,"facialHair":{"moustache":0.4,"beard":0.4,"sideburns":0.1},"glasses":"NoGlasses","emotion":{"anger":0.0,"contempt":0.0,"disgust":0.0,"fear":0.0,"happiness":0.0,"neutral":0.999,"sadness":0.001,"surprise":0.001},"blur":{"blurLevel":"high","value":1.0},"exposure":{"exposureLevel":"goodExposure","value":0.59},"noise":{"noiseLevel":"medium","value":0.34},"makeup":{"eyeMakeup":false,"lipMakeup":false},"accessories":[],"occlusion":{"foreheadOccluded":false,"eyeOccluded":false,"mouthOccluded":false},"hair":{"bald":0.82,"invisible":false,"hairColor":[]}}}]
```

### Speech-To-Text API ###
*Cloud Speech-to-Text API* je **Cloudová služba**, poskytovaná firmou **Google**. Pomocou tejto služby vieme prekladať hovorenú reč do písanej. Môžeme vytvárať dlhé, krátke prepisy alebo kontinuálne prepisy v reálnom čase.

<p align="center">
  <img src="https://blog.pythian.com/wp-content/uploads/serverless-pipeline-arch.png">
</p>


>Link na video: https://youtu.be/Z2pyW2pmPQI
