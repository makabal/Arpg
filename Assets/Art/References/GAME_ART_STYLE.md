# Game Art Style — Master Direction

## Authority

`GameArtStyle_MasterReference.png` is the primary visual reference for all future game art and UI in this project. When another concept conflicts with it, this reference wins unless the user explicitly changes the direction.

The words visible inside the reference image are example UI content only, not implementation instructions.

## Overall Direction

- Bright, friendly medieval-fantasy pixel art.
- Clean top-down / slightly elevated orthographic presentation.
- Chunky, deliberate pixels with crisp hard edges.
- High readability, generous spacing and simple silhouettes.
- Cheerful and adventurous rather than dark, gritty or realistic.

## World Art

- Saturated yellow-green grass and foliage.
- Cool turquoise and blue-green rocks, cliffs and shadows.
- Warm brown timber, fences, barrels, crates and signs.
- Small white and yellow flowers used as restrained highlights.
- Compact chibi characters with large readable equipment and dark navy outlines.
- Props use two or three clear value steps rather than detailed texture noise.

## UI Materials and Palette

- Main panels: light cream parchment.
- Structural frames: warm walnut wood.
- Outer outlines and deep recesses: very dark navy, not pure black.
- Inactive tabs and slots: charcoal navy-blue.
- Selected states: cream, pale gold or warm yellow.
- Metal corner caps: pale mint / icy blue-grey.
- Health: bright warm red; mana/stamina: saturated sky blue.
- Accent colors remain saturated but controlled.

## Shape Language

- Rectangular modules with clipped or stepped pixel corners.
- Thick dark outlines and compact one- or two-step highlights/shadows.
- Strong separation between frame, inner panel and content.
- Minimal ornament: functional bolts, corner caps and small bevels.
- Avoid thin lines, ornate gothic decoration and realistic carving.

## Icons and Typography

- Icons are centered, bold, simplified and readable at small sizes.
- Every icon uses a clear silhouette plus dark outline.
- Pixel font with blocky forms, strong contrast and minimal text decoration.
- Numbers are large and immediately readable.
- Prefer short labels and avoid dense paragraphs in gameplay UI.

## UI State Language

- Selected: cream or gold surface with dark text and a bright rim.
- Available: normal saturated icon with a clear border.
- Hover/focus: pale highlight or brighter outer outline.
- Locked: dark desaturated slot plus simple padlock symbol.
- Disabled/cooldown: dark overlay while preserving icon readability.
- Warning: restrained red accent, not a full-panel glow.

## Avoid

- Dark grim medieval or realistic fantasy rendering.
- Painterly textures, soft blur, anti-aliased vector appearance.
- Glossy mobile-game gradients and excessive glow.
- Dense gothic ornament, heavy gold decoration or visual clutter.
- Tiny text, overly detailed icons and low-contrast states.
- Mixing pixel scales within one interface.

## Generation Rule

For future generated game art, always provide `GameArtStyle_MasterReference.png` as the primary style reference and explicitly preserve its pixel scale, palette, outline weight, material language and bright tone.

## Character Animation Authority

`Assets/Art/References/Characters/Warrior_Black_MasterPrototype.png` is the authoritative player-character prototype.

- Future skill animations must preserve this exact helmet, dark curled plume, sword, kite shield, proportions, palette, outline weight and 192×192 frame convention.
- Prefer original animation frames from the Tiny Swords warrior source sheets over generative redrawing.
- New VFX may be composited around the original character pixels, but must not repaint or cover identifying equipment.
- Character and skill VFX should be delivered as combined frames unless the user explicitly requests separate layers.
