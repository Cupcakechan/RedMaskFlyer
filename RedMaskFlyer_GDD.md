# 🦸 RED MASK FLYER — Game Design Document & Roadmap
**Version:** 1.1 (Revised — reflects current implementation)
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
| Animations | Keep simple — Animator Controller + basic clips, drag-and-drop frames |
| Version Control | Commit + push to `main` after each working feature |
| WebGL Build Output | `Builds/WebGL` · Compression Format = Disabled |
| Deploy | `update-RedMaskFlyer.bat` → `butler push "Builds/WebGL" mrcanela/red-mask-flyer:webgl --userversion %1` |

---

## 1. CONCEPT
A common medieval hero in a crude red mask has magically learned to flight — no jetpack, pure arcane energy. He soars through a dark forest sky, dodging walking enemies and hazards, collecting coins and life pickups, chasing a high score. Endless; only death ends a run.

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
| Release | Fall (gravity) |
| `A` / `←` | Walk left (ground only) |
| `D` / `→` | Walk right (ground only) |
*Unity 6 new Input System (`InputActionAsset`).*

---

## 4. CORE LOOP
```
[MainMenu] --START RUN--> [Gameplay] --lives = 0--> [GameOver] --Try Again--> [Gameplay]
                                                              --Main Menu--> [MainMenu]
```
Per frame: world scrolls left → hold = rise / release = fall → on ground walk L/R → enemies & obstacles spawn right, move left, despawn left → score = distance + coins → difficulty scales with distance.

---

## 5. WIN / LOSE
| Condition | Result |
|-----------|--------|
| Lives reach 0 | Game Over screen |
| New high score | Prompt name entry, store via PlayerPrefs |
| Distance milestones | Difficulty stage up (no win state — endless) |

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
| `ButtonContainer` | 1394 × 784 | Transparent PNG · 9-slice border 80/80/80/80 · Sliced |
| `bitknight-custom SDF` | — | TMP default font · Static atlas |

### Backgrounds / Tiles / Enemies / Collectibles / Audio
- Solid color sky `#020016` (no sprite) for menus; parallax layers in Gameplay (sky fill, large clouds, small clouds ×5, far/mid/close trees, ground tilemap).
- Ground & props from provided tile sheet(s) → Tilemap, 16 PPU.
- Enemies walk left, contact = −1 life.
- Coins (+10 score), Life pickup (+1 life, cap 5).
- BGM playlist loop + SFX (fly, land, coin, hit, life lost, game over, UI click).

---

## 7. SCENE 0 — MAIN MENU ✅ BUILT
| Element | Exact Value |
|---------|-------------|
| Camera Background | `#020016` |
| Canvas | Screen Space - Camera · Render Camera = Main Camera · Scale With Screen Size · Ref 960×540 · Match 0.5 |
| Background (Image) | Stretch-Both (Shift+Alt) · offsets 0 · color `#020016` |
| TitleCard (Image) | Top-Center (Shift+Alt) · W 520 H 293 · Pos 0, -30 · Preserve Aspect ✅ |
| ButtonPanel | Center-Middle (Shift+Alt) · W 280 H 320 · Pos 0, -60 · Vertical Layout Group · Spacing 12 · Middle Center · Control Width ✅ · Force Expand Width ✅ |
| Buttons ×4 | Height 60 · Image = ButtonContainer (Sliced) · Transition Sprite Swap |
| Button Text (TMP) | Font `bitknight-custom` · Size 22 · `#FFE066` · Bold · Center+Middle |
| Buttons | START RUN · HOW TO PLAY · HIGH SCORE · QUIT |
| Script | `MainMenuUI.cs` on Canvas · OnStartRun / OnHowToPlay / OnHighScore / OnQuit |

---

## 8. SCENE 1 — GAMEPLAY (planned exact values)
| Element | Value |
|---------|-------|
| Camera | Orthographic · size 5 · fixed (world scrolls, camera does not move horizontally) |
| Player Start | Pos X −4 · Y at ground level |
| Rigidbody2D | Dynamic · Gravity Scale 3.5 (tunable) |
| Colliders | CapsuleCollider2D body · small BoxCollider2D feet trigger (ground check) |
| Ground Walk Speed | 3 u/s (tunable) |
| Fly | Upward velocity override while input held (tunable) |
| Invincibility | 1.5 s blink after hit |
| World Speed | start 5.0 u/s · global `WorldManager.Speed` |
| Ground | Tilemap strip · world Y 0 · player feet rest ~Y 0.5 |

### HUD (Screen Space Overlay)
| Element | Value |
|---------|-------|
| Hearts | Top-left · 32×32 px each |
| Score | Top-center · TMP size 36 · `#FFFFFF` |
| Best | Below score · TMP size 24 · `#FFD700` |

### Progressive Difficulty (DifficultySettings ScriptableObject)
| Stage | Distance | World Speed | Spawn Mult |
|-------|----------|-------------|-----------|
| 1 | 0–500 | 5.0 | 1.0× |
| 2 | 500–1200 | 6.0 | 1.3× |
| 3 | 1200–2500 | 7.0 | 1.6× |
| 4 | 2500–4500 | 8.0 | 2.0× |
| 5+ | 4500+ | 9.0+ | 2.5×+ (capped) |

---

## 9. SCENE 2 — GAME OVER (planned)
| Element | Value |
|---------|-------|
| Background | `#020016` |
| "GAME OVER" | TMP size 64 · `#FF2222` |
| Score | TMP size 36 |
| Best | TMP size 28 · `#FFD700` |
| Buttons | TRY AGAIN → Gameplay · MAIN MENU → MainMenu (ButtonContainer sliced, same style as menu) |
| New best | Flash "NEW BEST!" + name entry |

---

## 10. PROJECT STRUCTURE
```
Assets/
├── _Scenes/        MainMenu · Gameplay · GameOver
├── _Scripts/       Core · Player · World · Entities · UI · Audio
├── _Sprites/       Player · Backgrounds · Tileset · Enemies · Collectibles · UI
├── _Audio/         Music · SFX
├── _Prefabs/
├── _ScriptableObjects/  DifficultySettings.asset
├── _Animations/    Player/ (Idle, Walk, Fly, Hurt, Dead + Player.controller)
├── _Tilemaps/
├── _Fonts/         bitknight-custom.ttf + bitknight-custom SDF.asset
└── _VFX/           MagicalAura · WindStreaks (polish phase)
```

---

## 11. DEVELOPMENT ROADMAP

### ✅ COMPLETED
- [x] Unity 6 URP 2D project · WebGL platform · Gamma · 960×540
- [x] Full folder structure
- [x] 3 scenes created & ordered (MainMenu=0)
- [x] GitHub repo live, pushing to `main`
- [x] UI art imported (TitleCard, ButtonContainer 9-sliced)
- [x] `bitknight-custom` TMP font asset (Static) set as default
- [x] **Main Menu scene fully built + button wiring (`MainMenuUI.cs`) tested**
- [x] Player spritesheet imported & sliced (16 PPU, Point, None)

### ▶️ IN PROGRESS — Step: Player Animations & Animator
- [ ] Create Player GameObject + 5 animation clips (drag-and-drop)
- [ ] Animator parameters + transitions (Idle ↔ Walk ↔ Fly ↔ Hurt ↔ Dead)

### CORE LOOP (one feature at a time)
- [ ] Player movement: hold-to-fly + ground walk + sprite flip on turn (`PlayerController.cs`)
- [ ] Tilemap ground
- [ ] World scroller (obstacles move left) (`WorldScroller.cs`)
- [ ] Parallax background — all layers (`ParallaxLayer.cs`)
- [ ] Enemy walker + obstacle spawning + object pooling
- [ ] Lives system (3 hearts, damage, invincibility, HUD)
- [ ] Score system (distance + coins, HUD)
- [ ] High score persistence (PlayerPrefs, named entries) → shown on Main Menu
- [ ] Game Over scene + full scene flow
- [ ] Progressive difficulty (DifficultySettings SO)
- [ ] Coins + Life pickups
- [ ] BGM playlist + all SFX

### BUILD & DEPLOY (vertical slice complete)
- [ ] WebGL build (Compression Disabled) → test in browser
- [ ] `update-RedMaskFlyer.bat` → first itch.io publish

### POLISH (only after vertical slice playable)
- [ ] **Main Menu buttons feel "alive" — hover/press scale + color feedback** *(user request — locked)*
- [ ] Magical Flight VFX (aura + wind streaks, tunable)
- [ ] Pause Menu · screen shake on damage · camera vertical soft-follow
- [ ] Stage transition title cards · animated menu title
- [ ] Mobile on-screen controls for WebGL
- [ ] Stretch: extra enemy types · shield/speed power-ups · combo multiplier · leaderboard UI

---

## 12. GIT WORKFLOW
Commit after each working feature, push to `main`:
```bash
git add .
git commit -m "feat: <feature>"
git push origin main
```
Convention: `feat:` `fix:` `chore:` `refactor:` `art:`

---

## 13. DEPLOY SCRIPT — `update-RedMaskFlyer.bat` (root of project)
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
*Version 1.1 — Revised to reflect actual implementation. All values tunable via Inspector unless marked fixed.*
