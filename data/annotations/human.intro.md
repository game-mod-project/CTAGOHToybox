이 그룹은 보병이다.

| 위치 | 내용 | 텍스트 파일 |
|---|---|---|
| `set/breed/**/*.set` | 보병 정의. 파일 하나가 유닛 하나 | 5,083 |
| `set/breed/**/*.inc` | 능력치 매크로. 폴더마다 `ability.inc`가 있고 `set/breed/presets/perks/`의 공용 정의를 include한다 | 43 |
| `entity/humanskin/**/*.def` | 외형 엔티티. 대부분 모델 파일 이름만 가리킨다 | 3,667 |
| `entity/humanskin/**/*.mdl`, `*.mtl` | 모델 구조와 재질 | 4,799 |

보병 하나는 `{breed` 블록 하나로 정의된다. `skin`이 외형 엔티티를, `armors/head`와 `inventory/item`이 `set/stuff`의 아이템을 가리킨다. 능력치는 `perks`와 `veterancy` 안에서 include와 매크로 호출로 채워지므로, 실제 값은 아래 "펼친 뒤의 키"에서 본다.
