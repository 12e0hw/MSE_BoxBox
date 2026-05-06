-- 기존 테스트 데이터 초기화
DELETE FROM game_records;
DELETE FROM users;

-- 테스트용 유저 데이터
INSERT INTO users (user_id, login_id, password, username) VALUES
(1, 'jaechan', '1234', 'Jaechan'),
(2, 'hyewon', '1234', 'Hyewon'),
(3, 'jiwoong', '1234', 'Jiwoong'),
(4, 'jaehyuk', '1234', 'Jaehyuk');

-- 테스트용 게임 기록 데이터
INSERT INTO game_records (record_id, user_id, stage_id, points, achieved_at) VALUES
(1, 1, 1, 50, TIMESTAMP '2026-04-13 10:00:00'),
(2, 1, 2, 82, TIMESTAMP '2026-04-13 10:10:00'),

(3, 2, 1, 70, TIMESTAMP '2026-04-13 10:05:00'),
(4, 2, 2, 65, TIMESTAMP '2026-04-13 10:20:00'),

(5, 3, 1, 68, TIMESTAMP '2026-04-13 10:03:00'),
(6, 3, 2, 60, TIMESTAMP '2026-04-13 10:15:00'),

(7, 4, 1, 40, TIMESTAMP '2026-04-13 10:01:00'),
(8, 4, 2, 75, TIMESTAMP '2026-04-13 10:12:00');