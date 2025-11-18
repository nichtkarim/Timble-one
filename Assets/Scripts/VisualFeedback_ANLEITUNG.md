# 🎨 VISUELLES FEEDBACK SYSTEM - SETUP ANLEITUNG

## 📦 Was wurde erstellt:

### 1. **VisualFeedbackManager.cs**
Zentrales System für alle visuellen Effekte:
- ✅ Screen Shake
- ✅ Partikel-Effekte
- ✅ Screen Flash (Rot/Grün)
- ✅ Slow Motion
- ✅ Object Pulse/Scale

### 2. **FloatingText.cs**
Schwebende Texte für Damage, Heal, Score:
- ✅ Damage Numbers (-1 HP)
- ✅ Heal Numbers (+1 HP)
- ✅ Score Display (+100)

### 3. **CupVisualEffects.cs**
Visuelle Effekte speziell für Cups:
- ✅ Glow/Leuchten
- ✅ Bounce/Hüpfen
- ✅ Rotation/Drehen
- ✅ Shake/Schütteln
- ✅ Wobble/Wackeln

---

## 🚀 SCHNELLSTART - SETUP IN 5 MINUTEN:

### SCHRITT 1: VisualFeedbackManager in Szene
1. **Create Empty GameObject** in Hierarchy
2. Umbenennen in "VisualFeedbackManager"
3. **Add Component** → VisualFeedbackManager
4. Im Inspector:
   - Main Camera: Ziehe deine Main Camera hier rein
   - (Optional) Partikel-Prefabs zuweisen

### SCHRITT 2: TextMeshPro installieren (falls nicht vorhanden)
1. Window → TextMeshPro → Import TMP Essential Resources
2. Fertig! (Wird für FloatingText benötigt)

### SCHRITT 3: CupVisualEffects zu Cups hinzufügen
1. Wähle alle Cup GameObjects aus
2. **Add Component** → CupVisualEffects
3. Fertig! (Funktioniert automatisch)

---

## 🎯 WAS PASSIERT JETZT AUTOMATISCH:

### ✅ Bei richtigem Cup:
- ✨ Grüne Partikel spawnen
- 💚 Grüner Screen Flash
- 📏 Cup wird größer (Pulse)
- 🎊 Konfetti-Effekt

### ❌ Bei falschem Cup:
- 💥 Rote Partikel spawnen
- ❤️ Roter Screen Flash
- 📳 Screen Shake (Kamera wackelt)
- 📏 Cup wird größer (Pulse)

### 💊 Bei Item-Nutzung:
- ✨ Gelbe Partikel am Item
- 📏 Item pulsiert
- 🔊 (Sound kann noch hinzugefügt werden)

### 💚 Bei Heilung:
- 💚 Cyan Partikel
- 📝 "+1 HP" Text erscheint
- ✨ Heilungs-Effekt

### 🐌 Bei Verlangsamungs-Item:
- ⏰ Slow Motion für 0.5 Sekunden
- ✨ Item-Effekt Partikel

---

## 🎨 OPTIONALE PARTIKEL-EFFEKTE (Für besseres Aussehen):

### Partikel-Prefabs erstellen:

#### 1. Richtiger Cup Partikel (Grün/Gold):
1. Rechtsklick Hierarchy → Effects → Particle System
2. Umbenennen in "CorrectCupParticles"
3. Inspector Einstellungen:
   ```
   Main:
   - Start Color: Grün/Gold Gradient
   - Start Size: 0.3
   - Start Speed: 5
   - Start Lifetime: 1.5
   
   Emission:
   - Rate over Time: 0
   - Bursts: 30 particles at time 0
   
   Shape:
   - Shape: Sphere
   - Radius: 0.5
   ```
4. Als Prefab speichern (Assets/Prefabs/)
5. Zum VisualFeedbackManager zuweisen

#### 2. Falscher Cup Partikel (Rot):
- Gleich wie oben, aber mit roter Farbe

#### 3. Heal Partikel (Cyan):
- Gleich wie oben, aber mit Cyan/Grün Farbe

#### 4. Item Use Partikel (Gelb):
- Gleich wie oben, aber mit gelber Farbe

---

## ⚙️ EINSTELLUNGEN IM INSPECTOR:

### VisualFeedbackManager:

#### Screen Shake Settings:
- **Shake Duration**: 0.3s (Länge des Shake)
- **Shake Intensity**: 0.3 (Stärke des Shake)

#### Slow Motion Settings:
- **Slow Motion Scale**: 0.3 (30% Speed)
- **Slow Motion Duration**: 0.5s

#### Flash Settings:
- **Correct Flash Color**: Grün (RGB: 0, 255, 0)
- **Wrong Flash Color**: Rot (RGB: 255, 0, 0)
- **Flash Duration**: 0.2s

---

## 🎮 VERWENDUNG IN EIGENEM CODE:

```csharp
// Screen Shake auslösen
VisualFeedbackManager.Instance.ShakeCamera();
VisualFeedbackManager.Instance.ShakeCamera(0.5f, 0.3f); // Custom

// Partikel spawnen
VisualFeedbackManager.Instance.PlayCorrectCupEffect(position);
VisualFeedbackManager.Instance.PlayWrongCupEffect(position);
VisualFeedbackManager.Instance.PlayHealEffect(position);

// Screen Flash
VisualFeedbackManager.Instance.FlashCorrect(); // Grün
VisualFeedbackManager.Instance.FlashWrong(); // Rot

// Slow Motion
VisualFeedbackManager.Instance.TriggerSlowMotion();
VisualFeedbackManager.Instance.TriggerSlowMotion(1f, 0.5f); // Custom

// Object Pulse
VisualFeedbackManager.Instance.PulseObject(transform, 1.5f, 0.3f);

// Kombinierte Effekte
VisualFeedbackManager.Instance.TriggerCorrectCupFeedback(position, cupTransform);
VisualFeedbackManager.Instance.TriggerWrongCupFeedback(position, cupTransform);
```

### FloatingText verwenden:

```csharp
// Damage Text
FloatingText.CreateDamageText(1, position);

// Heal Text
FloatingText.CreateHealText(1, position);

// Score Text
FloatingText.CreateScoreText(100, position);

// Custom Text
FloatingText.Create("PERFECT!", position, Color.yellow, 1.5f);
```

### CupVisualEffects verwenden:

```csharp
CupVisualEffects cupFX = cup.GetComponent<CupVisualEffects>();

// Glow aktivieren
cupFX.EnableGlow();
cupFX.DisableGlow();
cupFX.GlowTemporary(2f); // Für 2 Sekunden

// Bounce
cupFX.Bounce();

// Rotation
cupFX.SpinOnce();
cupFX.Spin(180f); // 180 Grad

// Shake
cupFX.Shake(0.5f, 0.1f);

// Wobble
cupFX.Wobble(0.5f);

// Highlight (Glow + Bounce)
cupFX.Highlight(2f);
```

---

## 🔥 ERWEITERTE FEATURES:

### Ball leuchten lassen während Shuffle:
```csharp
// In MainGameLogic.cs bei shuffleCups():
CupVisualEffects correctCupFX = correctCup.GetComponent<CupVisualEffects>();
if (correctCupFX != null)
{
    correctCupFX.EnableGlow();
}

// Nach Shuffle:
correctCupFX.DisableGlow();
```

### Konfetti bei Sieg:
```csharp
// Bei Game Over (Spieler gewinnt):
if (Player.getCurrentHealth() > 0)
{
    VisualFeedbackManager.Instance.PlayCorrectCupEffect(Camera.main.transform.position + Vector3.forward * 2);
    VisualFeedbackManager.Instance.FlashCorrect();
}
```

### Trail-Effekt bei Cup-Bewegung:
1. Add Component → Trail Renderer zu Cup
2. Einstellungen:
   - Time: 0.3
   - Width: 0.2 → 0
   - Color: Weiß → Transparent

---

## 🐛 TROUBLESHOOTING:

### Problem: Keine Partikel sichtbar
**Lösung**: 
- Prüfe ob Position korrekt ist
- Partikel-Layer in Kamera sichtbar?
- Fallback-Partikel werden automatisch erstellt (einfache Version)

### Problem: Screen Shake funktioniert nicht
**Lösung**:
- Main Camera im VisualFeedbackManager zugewiesen?
- Kamera ist nicht als Child eines anderen Objekts?

### Problem: FloatingText nicht sichtbar
**Lösung**:
- TextMeshPro importiert? (Window → TextMeshPro → Import TMP)
- Text zu klein? Position zu weit weg?

### Problem: Slow Motion beeinflusst alles
**Lösung**:
- Das ist normal! Time.timeScale beeinflusst alle Time.deltaTime
- Wenn bestimmte Dinge nicht beeinflusst werden sollen: Time.unscaledDeltaTime verwenden

### Problem: Flash Overlay nicht sichtbar
**Lösung**:
- Canvas wird automatisch erstellt
- Prüfe Canvas Sorting Order (sollte 9999 sein)

---

## 🎯 PERFORMANCE TIPPS:

1. **Object Pooling für Partikel** (falls viele spawnen):
   - Partikel wiederverwenden statt immer neu erstellen
   
2. **Screen Shake begrenzen**:
   - Nicht zu oft gleichzeitig auslösen
   
3. **FloatingText Limit**:
   - Max 10-20 gleichzeitig

4. **Glow Materials cachen**:
   - Material einmal erstellen und wiederverwenden

---

## 📋 CHECKLISTE - HABE ICH ALLES?

- [ ] VisualFeedbackManager GameObject in Szene
- [ ] Main Camera zugewiesen
- [ ] TextMeshPro importiert
- [ ] CupVisualEffects zu Cups hinzugefügt
- [ ] (Optional) Partikel-Prefabs erstellt
- [ ] Test im Play Mode gemacht

---

## 🎨 NÄCHSTE SCHRITTE:

1. **Audio hinzufügen** (AudioManager.cs erstellen)
2. **UI-Animationen** (Health Bar Pulse, Score Counter)
3. **Mehr Partikel-Variationen** (Sterne, Rauch, Funken)
4. **Post-Processing** (Bloom, Color Grading bei Effekten)
5. **Combo-System** (Bei mehreren richtigen Cups hintereinander)

---

🎮 **VIEL SPASS BEIM TESTEN!**

Probiere verschiedene Einstellungen aus um den perfekten Look zu finden!
