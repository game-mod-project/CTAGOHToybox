## 메모

- **유닛을 추가하지 않고 기존 것을 고친다.** 바닐라 텍스트 파일 51개의 내용이 다르다: `set/target` 16개(표적·탄 선택 규칙), `set/stuff` 14개, `entity/-vehicle` 13개, `set/vision` 3개 등. 유닛 파일로는 차량 12개, 아이템 14개, 보병 2개다.
- **새 파일은 주로 날씨다.** 새 텍스트 파일 58개 중 55개가 `set/environment` 아래다. 설명의 날씨 전환 기능과 맞는다.
- **새 구성 10행, 블록 이름 6종.** 패널 버튼 3개(`ai_auto_move`, `medic_switch`, `weather_switch`), 발각 항목 2개(`firing_binocular`, `firing_searchlight`), `look_around`.
- **스크립트 기능은 이 분석에 잡히지 않는다.** 자동 포격·자동 위생병 같은 기능은 맵에 심는 스크립트로 구현된다고 설명에 적혀 있다. 맵 파일은 범위 밖이다.
- **다른 모드와 겹치는 파일.** MACE와 6개(`set/panel_control/right_main.set`, `right_special.set`, `set/target/ammo.set`, `set/vision/settings.set`, `set/vision/spotting.inc`, `set/stuff/shell/bulletger_105.heat.ammo`), Fusion과 1개(`set/stuff/special/searchlight`). 모두 내용이 다르므로 로드 순서에 따라 한쪽의 변경이 사라진다.
