이 그룹은 위 네 그룹에 속하지 않는 `set/`, `script/`, `properties/`의 모든 텍스트 파일이다. 유닛이 include하는 공통 속성, 편제와 비용, 게임 규칙, 상호작용 스크립트, 인터페이스 등록 정보가 섞여 있다.

| 위치 | 내용 |
|---|---|
| `properties/*.ext`, `*.inc` | 엔티티 공통 속성과 매크로. 차량·아이템 `.def`가 include한다 |
| `set/multiplayer`, `set/dynamic_campaign` | 편제, 비용, 연구 트리, 게임 모드 |
| `set/interaction_entity`, `set/interaction_terrain` | 피격·파괴 등 이벤트 처리 |
| `set/blast.inc`, `set/ballistics.set`, `set/damage.set` | 폭발·탄도·피해 계산 |
| `set/vision`, `set/ability`, `set/difficulty` | 시야, 능력, 난이도 |
| `set/registry`, `set/talk`, `set/environment` 등 | 인터페이스, 음성, 환경 |

이 그룹은 유닛 단위로 펼치지 않으므로 "펼친 뒤의 키"가 없다. 사람이 뜻을 살펴본 키도 없다. 마지막 열의 원문 주석이 가장 쓸모 있는 단서다: `properties/`의 파일에는 개발자가 키마다 단위와 뜻을 적어 둔 곳이 많다.
