-- 기존 테스트 데이터 초기화
DELETE FROM game_records;
DELETE FROM users;

-- 테스트용 유저 데이터
-- 현재 코드 기준 로그인 테스트 계정
-- loginId / password
-- jaechan / 1234
-- hyewon  / 1234
-- jiwoong / 1234
-- jaehyuk / 1234

-- INSERT INTO users (user_id, password, username) VALUES
-- (1, '1234', 'Jaechan'),
-- (2,  '1234', 'Hyewon'),
-- (3, '1234', 'Jiwoong'),
-- (4, '1234', 'Jaehyuk');

-- 테스트용 게임 기록 데이터
-- 목적:
-- 1) 전체 리더보드 테스트
-- 2) 스테이지별 리더보드 테스트
-- 3) 유저 최고 점수 조회 테스트
-- 4) 한 유저의 여러 기록 중 최고 점수만 뽑히는지 확인

-- INSERT INTO game_records (record_id, user_id, stage_id, points, achieved_at) VALUES
-- (1, 1, 1, 50, TIMESTAMP '2026-04-13 10:00:00'),
-- (2, 1, 2, 82, TIMESTAMP '2026-04-13 10:10:00'),

-- (3, 2, 1, 70, TIMESTAMP '2026-04-13 10:05:00'),
-- (4, 2, 2, 65, TIMESTAMP '2026-04-13 10:20:00'),

-- (5, 3, 1, 68, TIMESTAMP '2026-04-13 10:03:00'),
-- (6, 3, 2, 60, TIMESTAMP '2026-04-13 10:15:00'),

-- (7, 4, 1, 40, TIMESTAMP '2026-04-13 10:01:00'),
-- (8, 4, 2, 75, TIMESTAMP '2026-04-13 10:12:00');

-- --------------------------------------------
-- 선택: 동점 처리 테스트용 데이터
-- 현재 서비스는 points DESC 기준만 확실하고,
-- achieved_at 동점 처리 규칙은 아직 추가 전이면 순서가 보장되지 않을 수 있음
-- 동점 처리 로직을 넣은 뒤 아래 예시를 참고해서 별도로 테스트
-- --------------------------------------------
-- INSERT INTO game_records (record_id, user_id, stage_id, points, achieved_at) VALUES
-- (9, 2, 2, 75, TIMESTAMP '2026-04-13 10:25:00'),
-- (10, 3, 2, 75, TIMESTAMP '2026-04-13 10:05:00');