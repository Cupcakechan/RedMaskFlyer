# 🦸 RED MASK FLYER — Game Design Document & Roadmap
**Version:** 1.2 (Revised — reflects current implementation as of this update)
**Engine:** Unity 6 (URP 2D)
**Genre:** Endless Side-Scroller / Infinite Flyer
**Art Style:** 16-bit Pixel Art — Medieval Fantasy Superhero
**Platform Target:** WebGL (itch.io) + optional Windows
**itch.io Slug:** `mrcanela/red-mask-flyer`
**GitHub Repo:** `Cupcakechan/RedMaskFlyer` (push to `main` only — no dev branch)
**Project Path:** `C:\Users\danie\Documents\Unity Projects\RedMaskFlyer`

---

## 0. PERMANENT RULES & STANDING REMINDERS
*(Checked every response — never silently assumed)*

| Rule | Locked Value |
|------|-------------|
| Engine | Unity 6 only |
| Entry Scene | `MainMenu` is ALWAYS Build index 0 |
| Build Order | 0 = MainMenu · 1 = Gameplay · 2 = GameOver |
| Build Target | **WebGL** (platform already switched) |
| Color Space | Gamma |
| Resolution | 960 × 540 |
| Pixels Per Unit | **16 for EVERY sprite** (player, tiles, BG, enemies, coins) |
| Filter Mode | Point (no filter) — all sprites |
| Compression | None — all sprites |
| Text | TextMeshPro only · default font = `bitknight-custom SDF` (Static atlas) |
| Pace | ONE core feature at a time · vertical slice playable BEFORE polish |
| New feature rule | Present 2–3 options (simple → complex), recommend one, WAIT for explicit pick before coding |
| Debugging rule | Deep step-by-step analysis first · one focused fix at a time · ask for specifics, never assume user error |
| Animations | Keep simple — Animator Controller + basic clips, drag-and-drop frames |
| Version Control | Commit + push to `main` after each working feature |
| WebGL Build Output | `Builds/WebGL` · Compression Format = Disabled |
| Deploy | `update-RedMaskFlyer.bat` → `butler push "Builds/WebGL" mrcanela/red-mask-flyer:webgl --userversion %1` |

---

## 0.5 CURRENT STATUS SNAPSHOT
**The vertical slice is fully playable.** Main Menu → Gameplay → Game Over loop works end-to-end, with biomes, parallax (incl. drifting clouds), pooled hazards/enemies, the Goblin Archer event, lives + distance scoring, a Top-5 high-score table with initials entry, a pause menu, music + core SFX, and flight juice (velocity tilt + magic sparkle trail).

**Biggest remaining milestone:** first WebGL build + butler deploy to itch.io (not yet done).
**Still genuinely "core" (not polish):** coins + life pickups, and progressive difficulty (`WorldManager.Speed` is currently a flat 5).

---

## 1. CONCEPT
A common medieval hero in a crude red mask has magically learned to fly — no jetpack, pure arcane energy. He soars through a dark forest sky, dodging walking enemies and hazards, chasing a high score. Endless; only death ends a run.

**Tone:** Scrappy heroism. Not a polished superhero — someone who just discovered flight and ran with it.

---

## 2. PURPOSE & AUDIENCE
| Item | Detail |
|------|--------|
| Purpose | Game #2 of the 20 Games Challenge — master 2D endless runner architecture, parallax, object pooling, progressive difficulty |
| Audience | Casual retro players · portfolio showcase |
| Session | 2–10 min per run |

---

## 3. CONTROLS
| Input | Action |
|-------|--------|
| `Space` / `W` / `↑` / Left-Click — **Hold** | Magical flight (rise) |
| Release | Fall (gravity-like accel) |
| `Esc` | Toggle Pause |
*Unity 6 new Input System. Horizontal position is fixed (the world scrolls); grounded state auto-runs the Walk animation.*

---

## 4. CORE LOOP
```
[MainMenu] --START RUN--> [Gameplay] --lives = 0--> [GameOver] --Try Again--> [Gameplay]
                                                              --Main Menu--> [MainMenu]
```
Per frame: world scrolls left → hold = rise / release = fall → enemies & obstacles spawn right, move left, despawn left → score = distance (+ coins, planned) → difficulty scales with distance (planned).

---

## 5. WIN / LOSE
| Condition | Result |
|-----------|--------|
| Lives reach 0 | Game Over screen |
| Run cracks the Top 5 | Initials entry prompt, stored via PlayerPrefs (JSON table) |
| Distance milestones | Difficulty stage up (planned — no win state, endless) |

---

## 6. CONFIRMED ASSET SPECS

### Player — `Hero_Super.png`
| Setting | Value |
|---------|-------|
| Sprite Mode | Multiple |
| PPU | 16 · Frame size 16 × 20 |
| Filter | Point · Compression None · Mesh Full Rect |
| Order in Layer | 10 |

**Sliced frame ranges (confirmed):**
| Animation | Frames | Clip Name |
|-----------|--------|-----------|
| Idle | `Hero_Super_1` → `_6` (6) | `Player_Idle` |
| Walk | `Hero_Super_7` → `_11` (5) | `Player_Walk` |
| Jump (Fly) | `Hero_Super_36` → `_41` (6) | `Player_Fly` |
| Hurt | `Hero_Super_48` → `_52` (5) | `Player_Hurt` |
| Dead | `Hero_Super_53` → `_59` (7) | `Player_Dead` |

### UI (imported)
| Asset | Source Size | Notes |
|-------|-------------|-------|
| `TitleCard` | 1394 × 784 | Transparent PNG · displayed 520 × 293 |
| `ButtonContainer` | 1394 × 784 | Transparent PNG · 9-slice border 80/80/80/80 · **must be Image Type = Sliced, Preserve Aspect OFF, Fill Center ON** to fill its rect |
| `bitknight-custom SDF` | — | TMP default font · Static atlas |

### Audio
- **Music:** BGM playlist (shuffled, loops) via `AudioManager`.
- **SFX implemented:** UI button click · **fly** (once on flight-start edge) · **hurt** (every damaging hit) · **enemy noise** (random clip per walking enemy on spawn).
- **SFX still planned:** land, coin pickup, life pickup, game-over sting, biome-transition cue.

### Backgrounds / Tiles / Enemies / Collectibles
- Menu sky: solid `#020016`. Gameplay: parallax layers (Far/Near) + drifting clouds (white `PS_Clouds` / purple `PS_Space_Clouds`).
- Ground from tile sheet → seamless dual-chunk Tiled scroller, 16 PPU.
- Enemies walk/fly left; contact = −1 life.
- **Coins (+score, planned)**, **Life pickup (+1 life, planned)**.

---

## 7. SCENE 0 — MAIN MENU ✅ BUILT
| Element | Exact Value |
|---------|-------------|
| Camera Background | `#020016` |
| Canvas | Screen Space - Camera · Render Camera = Main Camera · Scale With Screen Size · Ref 960×540 · Match 0.5 |
| Background (Image) | Stretch-Both (Shift+Alt) · offsets 0 · color `#020016` |
| TitleCard (Image) | Top-Center (Shift+Alt) · W 520 H 293 · Pos 0, -30 · Preserve Aspect ✅ |
| ButtonPanel | Center-Middle (Shift+Alt) · W 280 H 320 · Pos 0, -60 · Vertical Layout Group · Spacing 12 · Middle Center · Control Width ✅ · Force Expand Width ✅ |
| Buttons ×4 | Height 60 · Image = ButtonContainer (Sliced) · Transition Sprite Swap · ButtonJuice |
| Button Text (TMP) | Font `bitknight-custom` · Size 22 · `#FFE066` · Bold · Center+Middle |
| Buttons | START RUN · HOW TO PLAY · HIGH SCORE · QUIT |
| Best text | `BEST: <n> m` (reads PlayerPrefs `BestScore`, synced to the high-score table's #1) |
| Script | `MainMenuUI.cs` on Canvas · OnStartRun / OnHowToPlay / OnHighScore / OnQuit (+ close handlers) |

### How To Play overlay ✅ BUILT
- Backdrop (`HowToPlayPanel`, Image, last sibling of Canvas) · Stretch-Both (Alt) · `#020016E6` · Raycast Target ✅.
- `Window` (ButtonContainer Sliced) · Center-Middle (Shift+Alt) · 600×460.
- `Title` "HOW TO PLAY" (Size 34, `#FFE066`), `Body` (controls + goal, rich text), `BackButton` (ButtonJuice → `OnCloseHowToPlay`).

### High Score overlay ✅ BUILT
- Same overlay pattern; `Window` 600×460 with `Title` "HIGH SCORES".
- `RowsArea` (Vertical Layout Group) with 5 rows; each row = `Label` (left, rank + initials) + `Score` (right, gold `#FFD700`).
- `HighScorePanel.cs` fills rows from `HighScores.GetEntries()` on `OnEnable`. Back button → `OnCloseHighScore`.

---

## 8. SCENE 1 — GAMEPLAY ✅ BUILT (slice)
| Element | Value |
|---------|-------|
| Camera | Orthographic · size 5 · 16:9 → ~17.78 × 10 units · fixed (world scrolls) |
| Player Start | Pos X ≈ −4 · X frozen |
| Flight | Manual accel (gravity off): riseAccel 30, fallAccel 20, maxRise 5, maxFall 7, ceiling maxY 3.7 |
| Flight juice | Velocity tilt (nose-up rising / nose-down falling) + magic sparkle trail (emits while fly held) |
| Lives | 5 · i-frame blink 1.5 s after a hit |
| World Speed | start 5.0 u/s · global `WorldManager.Speed` *(flat — difficulty ramp not yet implemented)* |
| Ground | Seamless dual-chunk Tiled scroller, biome-aware |

### HUD
| Element | Value |
|---------|-------|
| Hearts | Top-left · single Image swapped by lives count |
| Score | Distance in meters, TMP |
| Pause | `Esc` → `PauseMenu` (Time.timeScale 0/1; Resume / Restart / MainMenu) |

### World systems (built)
- `WorldManager` (Speed singleton) · `ScoreManager` (distance meters) · `RunData` (LastScore / IsNewBest).
- `BiomeManager`: starter biome + shuffled biomes loop · `biomeLength` 500 m · sky-color lerp · swaps spawner entries, ground, parallax sprites, and cloud set per biome.
- `BiomeData` (inline `[Serializable]`): entries, groundSprite, skyColor, parallaxFar/NearSprite, `biomeName`, `useSpaceClouds`, `spawnClouds`.
- Parallax: `ParallaxLayer` Far/Near (wrap + biome sprite swap); **Phase 3 clouds** — `CloudSpawner` + `CloudDrifter` (pooled, world-speed-relative drift, biome-aware white/space, per-biome `spawnClouds` toggle, sort order −15).
- Hazards/Enemies: `ObjectPool` (multi-prefab, recycle via SetActive) · `HazardSpawner` (weighted entries, mileage gating, warning blink) · Static / Walker / Flying spawners · per-biome obstacle pools + art · `Obstacle` (`extraSpeed` = 0 static, > 0 walker; walkers play a random spawn noise) · `ScrollingObject`.
- Goblin Archer event: `ArcherEvent` (timed) + `GoblinArcher` + `Arrow` (telegraphs, fires snapshot-aimed arrows). *(Obstacle script must NOT be on the archer prefab.)*

### Progressive Difficulty (DifficultySettings ScriptableObject) — ⚠️ PLANNED, NOT BUILT
| Stage | Distance | World Speed | Spawn Mult |
|-------|----------|-------------|-----------|
| 1 | 0–500 | 5.0 | 1.0× |
| 2 | 500–1200 | 6.0 | 1.3× |
| 3 | 1200–2500 | 7.0 | 1.6× |
| 4 | 2500–4500 | 8.0 | 2.0× |
| 5+ | 4500+ | 9.0+ | 2.5×+ (capped) |

---

## 9. SCENE 2 — GAME OVER ✅ BUILT
| Element | Value |
|---------|-------|
| Background | `#020016` |
| "GAME OVER" | TMP large red |
| Distance | `DISTANCE: <n> m` (from `RunData.LastScore`) |
| Best | `BEST: <n> m` (PlayerPrefs, synced to table #1) |
| Buttons | TRY AGAIN → Gameplay · MAIN MENU → MainMenu (ButtonContainer Sliced + ButtonJuice) |
| High-score entry | If `HighScores.Qualifies(score)`: show `EntryGroup` (prompt + TMP_InputField, 3 letters, auto-uppercase + Submit). On submit → `HighScores.Insert`, then `ResultText` shows `NEW HIGH SCORE — RANK #n!`. Non-qualifying runs skip the prompt. |

---

## 10. PROJECT STRUCTURE (scripts as built)
```
Assets/_Scripts/
├── Core/      WorldManager · ScoreManager · RunData · BiomeManager · HighScores
├── Player/    PlayerController (+Instance, flight, tilt, sparkle toggle) · PlayerHealth
├── World/     ObjectPool · HazardSpawner · GroundChunk · ParallaxLayer · BiomeData
│              · ArcherEvent · CloudSpawner · CloudDrifter · ScrollingObject
├── Entities/  Obstacle · Arrow · GoblinArcher
├── UI/        MainMenuUI · GameOverUI · PauseMenu · HighScorePanel · ButtonJuice · WarningBlink
├── Audio/     AudioManager (music playlist + PlaySFX/PlayFly/PlayHurt/PlayEnemyNoise)
└── Editor/    HighScoreDebug (menu: Log Table / Test Insert / Clear and Reseed)
```
Other folders: `_Scenes`, `_Sprites`, `_Audio` (Music/SFX), `_Prefabs`, `_VFX` (`MagicMote_Mat`), `_Animations`, `_Fonts`.

---

## 11. DEVELOPMENT ROADMAP

### ✅ COMPLETED
- [x] Unity 6 URP 2D · WebGL platform · Gamma · 960×540 · folder structure · 3 ordered scenes · GitHub (`main`)
- [x] UI art imported · `bitknight-custom` TMP font (Static)
- [x] Main Menu scene + button wiring (`MainMenuUI`)
- [x] **How to Play overlay panel**
- [x] **High Score overlay panel (Top-5 listing)**
- [x] Player spritesheet sliced · Animator (Idle/Walk/Fly/Hurt/Dead)
- [x] Player movement: hold-to-fly accel + ground auto-run (`PlayerController`)
- [x] **Flight feel: velocity tilt + magic sparkle trail (Particle System)**
- [x] Seamless biome-aware Tiled ground scroller
- [x] Parallax background — Far/Near layers (Phases 1 & 2)
- [x] **Parallax Phase 3 — drifting clouds (biome-aware, per-biome on/off)**
- [x] Enemy/obstacle spawning + object pooling (Static/Walker/Flying) · per-biome pools
- [x] Goblin Archer special event
- [x] Lives system (5 hearts, i-frames, HUD)
- [x] Score system (distance, HUD)
- [x] **High score persistence — Top-5 JSON table + initials entry on Game Over (`HighScores`)**
- [x] Game Over scene + full scene flow
- [x] Biome system (`BiomeManager` + `BiomeData`, sky-color lerp, content swaps)
- [x] BGM playlist (`AudioManager`)
- [x] **SFX: button click, fly, hurt, per-enemy spawn noise**
- [x] Pause menu (Esc, Time.timeScale)
- [x] Button juice on all buttons (menu, pause, game over, panel backs)

### ▶️ NEXT — Build & Deploy milestone
- [ ] **WebGL build (Compression Disabled) → test in browser**
- [ ] **`update-RedMaskFlyer.bat` → first itch.io publish**

### ◻️ STILL CORE (not polish)
- [ ] Coins + life pickups (coins double as currency for character unlocks later)
- [ ] Progressive difficulty (`DifficultySettings` SO — ramp `WorldManager.Speed` + spawn rate by distance)
- [ ] Remaining SFX: land · coin · life pickup · game-over sting · biome-transition cue

### ✨ POLISH (after the slice ships)
- [ ] Screen shake on damage
- [ ] Hit feedback: brief player flash + optional hit-stop
- [ ] Coin/score pickup pop ("+10") + sound
- [ ] Death feedback: slow-mo / flash / camera nudge
- [ ] Camera vertical soft-follow
- [ ] Biome transition title cards
- [ ] URP post-processing: bloom (makes the sparkle trail glow) + subtle vignette
- [ ] 2D lights for the dark-forest mood
- [ ] Wind streaks / speed lines at higher speeds
- [ ] Settings: music & SFX volume sliders + mute
- [ ] Scene fade transitions (fade to black)
- [ ] Animated menu title / floating hero · first-run "HOLD to fly" hint
- [ ] High Score panel: highlight the just-set entry · entry pop animation
- [ ] Mobile on-screen controls for WebGL

### 🚀 STRETCH
- [ ] Character unlocks via coins (meta-progression)
- [ ] Power-ups (shield / speed) · combo multiplier · extra enemy types · online leaderboard

---

## 12. GIT WORKFLOW
Commit after each working feature, push to `main`:
```bash
git add .
git commit -m "feat: <feature>"
git push origin main
```
Convention: `feat:` `fix:` `chore:` `refactor:` `art:` `polish:`

---

## 13. DEPLOY SCRIPT — `update-RedMaskFlyer.bat` (root of project) — ⚠️ NOT YET CREATED
```bat
@echo off
echo ========================================
echo Updating %~n0 on itch.io
echo ========================================
cd /d "C:\Users\danie\Documents\Unity Projects\RedMaskFlyer"
echo.
echo Pushing WebGL build to itch.io...
butler push "Builds/WebGL" mrcanela/red-mask-flyer:webgl --userversion %1
echo.
echo Done! Game updated on itch.io.
echo.
pause
```

---
*Version 1.2 — Revised to reflect actual implementation. All values tunable via Inspector unless marked fixed.*
