## 메모

- **유닛을 거의 건드리지 않는다.** 새 차량류 `.def` 5개는 모두 `entity/-vehicle/generic/special` 아래의 상자류(`inf_crate_...`)다. 보병과 아이템은 추가하지 않는다.
- **본체는 맵 파일이다.** 파일 552개 중 479개가 `map/multi`와 `map/single` 아래에 있고 바닐라의 같은 경로를 덮어쓴다(`override-unhashed` 480개의 대부분). 미션 파일에 메뉴를 심는 방식으로 보인다. 맵·미션 파일은 이 분석의 범위 밖이라 내용을 비교하지 않았다. `map/cheat/` 폴더도 들어 있다.
- **바뀐 바닐라 텍스트 파일 2개.** `set/interaction_entity`의 파일 하나와 `set/panel_control/right_special.set`이다. 뒤의 것은 MACE와 Auto Medic도 고치므로 세 모드가 같은 파일을 서로 다르게 가진다.
- **새 구성.** 블록 이름 `cell`(인벤토리 칸 위치)과 `change_paint_scheme_toggle`(패널 버튼), 매크로 `tex_mod`.
- 설명대로 바닐라 유닛용이다. MACE의 유닛은 이 모드의 메뉴에 없다. 2026-07에 만든 로컬 패치 모드(`mods\SpawnMenu_MACE_Fusion_Ground_Patch`)가 그 틈을 메우려던 것이다.
