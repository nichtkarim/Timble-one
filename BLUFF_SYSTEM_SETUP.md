# 🎲 Neues Bluff-System Setup Guide

## 🎯 System-Übersicht

### **WIE FUNKTIONIERT DAS NEUE SYSTEM:**

#### 1. **Dealer-Betrug (25% Chance)**
- Bei **jeder Runde** gibt es eine **feste 25% Chance**, dass der Dealer die Kugel entfernt
- Dies passiert **unabhängig von der Intuition** des Spielers  
- Wenn die Kugel entfernt wird → **KEIN** Becher ist richtig
- Ball wird visuell deaktiviert

#### 2. **Intuitions-Tipp System**
- **NUR wenn der Dealer tatsächlich geschummelt hat**, kann Spieler einen Tipp bekommen
- Chance auf Tipp hängt von **aktueller Intuition** ab:
  - 100% Intuition → 100% Chance den Betrug zu erkennen
  - 50% Intuition → 50% Chance den Betrug zu erkennen  
  - 0% Intuition → 0% Chance
- **Wenn Tipp erhalten:** 
  - Cyan Flashscreen erscheint
  - UI-Text: "💡 Deine Intuition sagt: Dealer hat geschummelt!"

#### 3. **Bluff-Button (IMMER verfügbar!)**
- Wird **IMMER** nach dem Mischen angezeigt (unabhängig vom Tipp!)
- Spieler kann **JEDE Runde** entscheiden ob er "Bluff" callt
- **Bluff bedeutet:** "Ich behaupte der Dealer hat geschummelt!"
- Button-Text: "🃏 BLUFF CALLEN"

#### 4. **Strafen-System**

##### ✅ **Bluff RICHTIG** (Dealer HAT tatsächlich geschummelt):
- Dealer verliert **2 Leben**
- **Grüner** Flashscreen
- Runde endet sofort
- Intuition bleibt unverändert
  
##### ❌ **Bluff FALSCH** (Dealer war ehrlich, Ball war im Spiel):
- Spieler verliert **2 Leben**
- Spieler verliert **20% Intuition** (zusätzliche Strafe!)
- **Roter** Flashscreen
- Runde endet sofort

#### 5. **Normale Cup-Wahl**
- Spieler kann auch **ohne Bluff** einen Becher wählen
- **Fall A:** Dealer NICHT geschummelt → Normale Spielmechanik
  - Richtiger Becher → Dealer verliert Leben
  - Falscher Becher → Spieler verliert Leben + 5% Intuition
- **Fall B:** Dealer HAT geschummelt (Spieler hat es nicht gemerkt)
  - Alle Becher sind falsch
  - Spieler verliert Leben + 5% Intuition
  - Dealer behält Leben

---

## ✅ Implementierungs-Checklist

### 📝 **C# Scripts (bereits implementiert):**

#### ✅ `IntuitionSystem.cs`
```csharp
// Neue Variablen:
public float ballRemovalChance = 0.25f;  // 25% feste Chance
private bool dealerCheatedThisRound = false;

// Neue Methoden:
ShouldRemoveBall()           // Feste 25% Chance, unabhängig von Intuition
GiveCheatingTip()            // Tipp OB geschummelt wurde (basierend auf Intuition)
HasCheatingTip()             // Prüft ob Tipp erhalten wurde
DealerCheatedThisRound()     // Status ob Dealer diese Runde geschummelt hat
CallBluff(out bool)          // Verarbeitet Bluff, gibt zurück ob richtig war

// Strafen:
- Kontinuierlich: -1% Intuition pro Sekunde
- Falscher Cup: -5% Intuition
- Falscher Bluff: -20% Intuition
```

#### ✅ `BluffButton.cs`
```csharp
// UI-Manager für Bluff-Interaktion
ShowBluffButton()            // Zeigt Button (IMMER nach Mischen!)
HideBluffButton()            // Versteckt Button
ShowTipInfo()                // Zeigt "Dealer hat geschummelt!" Text
HideTipInfo()                // Versteckt Tipp-Text
OnBluffButtonClicked()       // Callback zu MainGameLogic.OnBluffCalled()
```

#### ✅ `MainGameLogic.cs`
```csharp
// Integration im Spielablauf:
private bool dealerRemovedBallThisRound = false;

// VOR dem Mischen:
dealerRemovedBallThisRound = IntuitionSystem.Instance.ShouldRemoveBall();

// NACH dem Mischen:
IntuitionSystem.Instance.GiveCheatingTip();
BluffButton.Instance?.ShowBluffButton();  // IMMER anzeigen!

// Bluff-Verarbeitung:
OnBluffCalled()              // Public Methode für Button-Callback
HandleBluff()                // Verarbeitet Bluff-Logik, gibt Leben ab, beendet Runde
```

#### ✅ `VisualFeedbackManager.cs`
```csharp
FlashScreen(Color color, float intensity, float duration)

// Verwendet für:
- Cyan: Tipp erhalten (Dealer hat geschummelt!)
- Grün: Bluff richtig
- Rot: Bluff falsch
```

---

## 🎨 Unity Editor Setup (TO DO)

### 1️⃣ **Bluff-Button Panel erstellen**

**Hierarchy:**
```
Canvas
└── BluffButtonPanel (Panel)
    └── BluffButton (Button - TextMeshPro)
        └── Text (TMP)
```

**Settings:**
- `BluffButtonPanel`:
  - Position: Unten rechts (z.B. X: -150, Y: 100)
  - Width: 200, Height: 60
  - Color: Semi-transparent Orange/Red (Warning)
  
- `BluffButton`:
  - Text: "🃏 BLUFF CALLEN"
  - Font Size: 18-20
  - Button Color: Orange (Normal), Red (Highlighted)

### 2️⃣ **Tip-Info Panel erstellen**

**Hierarchy:**
```
Canvas
└── TipInfoPanel (Panel)
    └── TipInfoText (Text - TextMeshPro)
```

**Settings:**
- `TipInfoPanel`:
  - Position: Oben mittig (X: 0, Y: -80)
  - Width: 600, Height: 80
  - Color: Semi-transparent Cyan
  
- `TipInfoText`:
  - Text: "💡 Deine Intuition sagt: Dealer hat geschummelt!"
  - Font Size: 24
  - Color: Bright Cyan
  - Alignment: Center + Middle
  - Font Style: Bold

### 3️⃣ **BluffButton Script zuweisen**

**Hierarchy:**
```
Scene
└── BluffButtonManager (Empty GameObject)
```

**Inspector:**
1. Add Component → `BluffButton.cs`
2. Drag & Drop References:
   - **Bluff Button:** → `BluffButton` (Button component)
   - **Button Text:** → `BluffButton/Text (TMP)`
   - **Button Panel:** → `BluffButtonPanel` (GameObject)
   - **Tip Info Panel:** → `TipInfoPanel` (GameObject)
   - **Tip Info Text:** → `TipInfoText` (TextMeshProUGUI)

### 4️⃣ **IntuitionSystem Einstellungen prüfen**

**Inspector (IntuitionSystem GameObject):**
```
Ball Removal Chance: 0.25          (25%)
Intuition Loss Per Second: 1.0     (1%/sec)
Intuition Loss On Wrong Cup: -5    (5%)
Intuition Loss On Wrong Bluff: -20 (20%)
```

---

## 🧪 Test-Szenarien

### ✅ **Test 1: Dealer schummelt + Spieler bekommt Tipp**
1. Starte Spiel mit **100% Intuition**
2. Nach Mischen:
   - ✅ **Cyan Flash** sollte erscheinen (wenn Dealer geschummelt hat)
   - ✅ Text "Dealer hat geschummelt!" angezeigt
   - ✅ Bluff-Button sichtbar
3. Click Bluff-Button:
   - ✅ **Grüner Flash**
   - ✅ Dealer verliert 2 Leben
   - ✅ Runde endet sofort

### ✅ **Test 2: Dealer schummelt + Spieler bekommt KEINEN Tipp**
1. Setze Intuition auf **30%**
2. Nach Mischen:
   - ❌ Kein Cyan Flash (70% Chance Tipp zu verpassen)
   - ❌ Kein Tipp-Text
   - ✅ Bluff-Button trotzdem sichtbar!
3. Click Bluff-Button (ohne Tipp):
   - Wenn Dealer geschummelt hat: ✅ Grün, Dealer -2 HP
   - Wenn nicht: ❌ Rot, Spieler -2 HP, -20% Intuition

### ✅ **Test 3: Dealer schummelt NICHT**
1. Nach Mischen:
   - ❌ Kein Cyan Flash (Dealer hat nicht geschummelt)
   - ✅ Bluff-Button trotzdem sichtbar
2. Click Bluff-Button:
   - ❌ **Roter Flash** (falsche Anschuldigung)
   - ❌ Spieler verliert 2 Leben
   - ❌ Spieler verliert 20% Intuition

### ✅ **Test 4: Normale Cup-Wahl (kein Bluff)**
1. Ignoriere Bluff-Button
2. Wähle normalen Becher:
   - Wenn richtig: Dealer -1 HP
   - Wenn falsch: Spieler -1 HP, -5% Intuition
   - Wenn Dealer geschummelt hatte (alle falsch): Spieler -1 HP, -5% Intuition

---

## 🔧 Troubleshooting

### ❌ **Problem:** Bluff-Button erscheint nicht
**Lösung:** 
- Prüfe `BluffButton.Instance` ist nicht null
- Prüfe `buttonPanel` ist assigned im Inspector
- Console-Log: "🃏 Bluff-Button angezeigt" sollte erscheinen

### ❌ **Problem:** Tipp erscheint immer/nie
**Lösung:**
- Prüfe `ballRemovalChance = 0.25` in IntuitionSystem
- Console-Log: "🎲 Dealer schummelt? True/False" zeigt Status
- Console-Log: "💡 TIPP ERHALTEN!" oder "❌ Kein Tipp" zeigt Tipp-Status

### ❌ **Problem:** Falsche Leben-Abzüge
**Lösung:**
- `HandleBluff()` sollte `Dealer.takeDamage(2)` bei richtigem Bluff callen
- `HandleBluff()` sollte `Player.takeDamage(2)` bei falschem Bluff callen
- Prüfe dass `DealerCheatedThisRound()` korrekt zurückgibt

### ❌ **Problem:** Intuition-Verlust falsch
**Lösung:**
- Falscher Bluff: `-20%` via `CallBluff()` in IntuitionSystem
- Falscher Cup: `-5%` via `OnWrongCupSelected()`
- Kontinuierlich: `-1%/sec` via `Update()` Loop

---

## 📊 Balancing-Überlegungen

### **Aktuelle Werte:**
```
Dealer Schummeln: 25% Chance
Tipp bei Schummeln: Intuition% Chance
Bluff richtig: Dealer -2 HP
Bluff falsch: Spieler -2 HP + -20% Intuition
Intuition Verlust: 1%/sec, +5% pro falscher Cup
```

### **Anpassungs-Möglichkeiten:**
- **Zu einfach?** → Erhöhe `ballRemovalChance` auf 0.35 (35%)
- **Zu schwer?** → Reduziere Intuition-Verlust auf 0.5%/sec
- **Bluff zu brutal?** → Reduziere Strafe auf -15% statt -20%
- **Dealer stirbt zu schnell?** → Bluff gibt nur 1 Leben Schaden statt 2

---

## 📋 Quick Reference

### **Spieler-Entscheidungsbaum:**
```
Nach Mischen:
├─ Bluff-Button erscheint (IMMER)
├─ Cyan Flash + Text? → Dealer hat geschummelt!
│
├─ Option A: BLUFF CALLEN
│  ├─ Dealer hat geschummelt? → Grün, Dealer -2 HP ✅
│  └─ Dealer war ehrlich? → Rot, Spieler -2 HP + -20% Intuition ❌
│
└─ Option B: BECHER WÄHLEN
   ├─ Dealer schummelte? → Alle falsch, Spieler -1 HP
   ├─ Richtiger Becher? → Dealer -1 HP
   └─ Falscher Becher? → Spieler -1 HP + -5% Intuition
```

### **Code-Flow:**
```
1. StartRound()
2. Ball unter Becher platzieren
3. ShouldRemoveBall() → 25% Chance = Ball weg
4. ShuffleCups()
5. GiveCheatingTip() → Wenn geschummelt + Intuition hoch → Cyan Flash
6. ShowBluffButton() → IMMER anzeigen
7. Warte auf Input:
   → Bluff-Button? → HandleBluff() → Runde endet
   → Cup-Click? → Normale Mechanik → Runde endet
```

---

## 🎉 Fertig!

Das neue Bluff-System ist jetzt vollständig implementiert!

**Nächste Schritte:**
1. ✅ Führe Unity Editor Setup aus (Panels erstellen)
2. ✅ Teste alle 4 Szenarien
3. ✅ Balance adjustieren falls nötig
4. ✅ Visual Polish (Particle Effects, Sound, Animations)
