-- 기존 테스트 데이터 초기화
DELETE FROM game_records;
DELETE FROM users;

-- 테스트용 유저 데이터
INSERT INTO users (password, username) VALUES
('1234', 'Jaechan'),
('1234', 'Hyewon'),
('1234', 'Jiwoong'),
('1234', 'Jaehyuk');

-- 테스트용 게임 기록 데이터
INSERT INTO game_records (points, stage_id, achieved_at, user_id) VALUES
(50, 1, '2026-04-13 10:00:00', 1),
(40, 1, '2026-04-13 10:05:00', 2),
(30, 1, '2026-04-13 10:10:00', 3),
(20, 2, '2026-04-13 10:15:00', 4);