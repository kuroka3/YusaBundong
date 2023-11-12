# YusaBundong
유사 분동겜  
2022.3.12f1

# 채보 로딩 방식
appdata/local/YusaBundong/Songs 폴더에 채보를 넣어서 플레이  
아직 채보 임포트 기능 구현이 안됨.  
채보는 osu!mania 채보를 컨버트 해서 적용.

# 판정범위
Obvious ±80ms, Really ±120ms, May ±160ms, Hardly ±180ms, Miss -180~

# 구현 안된 기능
 - 곡 임포트
 - 게임종료 버튼
 - 키빔
 - 설정

# hi-speed 관련
롱노트 길이 계산하기 귀찮아서 선형회귀로 만들었더니 hi-speed 변형이 클수록 롱노트 길이 오차가 심함.  
디폴트 값에서 웬만하면 변경하지 마십셔