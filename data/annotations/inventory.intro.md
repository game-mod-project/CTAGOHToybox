이 그룹은 아이템의 3D 엔티티다.

| 위치 | 내용 | `.def` 수 |
|---|---|---|
| `entity/inventory/-gear_wearable` | 헬멧, 군장 | 1,475 |
| `entity/inventory/-ammo` | 탄약 | 452 |
| `entity/inventory/-weapon` | 무기 | 281 |
| `entity/inventory/-stuff`, `-tools`, `-medical` | 기타 | 45 |

`.def`는 `properties/stuff_*.ext`를 include하고, `{extender "stuff" {item "이름"}}`으로 `set/stuff`의 아이템과 이어진다. 수치는 거의 없고 충돌 볼륨과 모델 지정이 대부분이다.
