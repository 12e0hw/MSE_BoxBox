package com.boxbox.server.service;

import com.boxbox.server.dto.BestScoreResponse;
import com.boxbox.server.dto.LeaderboardItemResponse;
import com.boxbox.server.entity.GameRecord;
import com.boxbox.server.entity.User;
import com.boxbox.server.repository.GameRecordRepository;
import com.boxbox.server.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

@Service
@RequiredArgsConstructor
public class LeaderboardService {

    private final GameRecordRepository gameRecordRepository;
    private final UserRepository userRepository;

    public List<LeaderboardItemResponse> getLeaderboard() {
        List<GameRecord> allRecords = gameRecordRepository.findAll();
        return buildLeaderboard(allRecords);
    }

    public List<LeaderboardItemResponse> getStageLeaderboard(Integer stageId) {
        List<GameRecord> stageRecords = gameRecordRepository.findByStageId(stageId);
        return buildLeaderboard(stageRecords);
    }

    public BestScoreResponse getUserBestScore(Long userId) {
        User user = userRepository.findById(userId)
                .orElseThrow(() -> new IllegalArgumentException("존재하지 않는 userId입니다."));

        List<GameRecord> userRecords = gameRecordRepository.findByUserUserId(userId);

        int bestScore = userRecords.stream()
                .map(GameRecord::getPoints)
                .max(Integer::compareTo)
                .orElse(0);

        return new BestScoreResponse(
                user.getUserId(),
                user.getUsername(),
                bestScore
        );
    }

    // 리더보드 계산 로직
    private List<LeaderboardItemResponse> buildLeaderboard(List<GameRecord> records) {
        // key: userId, value: 그 유저의 최고 기록
        Map<Long, GameRecord> bestRecordByUser = new LinkedHashMap<>();

        for (GameRecord record : records) {
            Long userId = record.getUser().getUserId();

            // 유저 기록이 없으면 저장
            if (!bestRecordByUser.containsKey(userId)) {
                bestRecordByUser.put(userId, record);
                continue;
            }

            // 저장된 최고 기록과 비교
            GameRecord currentBest = bestRecordByUser.get(userId);
            
            // 현재 기록이 높으면 교체
            if (record.getPoints() > currentBest.getPoints()) {
                bestRecordByUser.put(userId, record);
            }
        }

        List<GameRecord> sortedBestRecords = new ArrayList<>(bestRecordByUser.values());
        sortedBestRecords.sort(Comparator.comparing(GameRecord::getPoints).reversed());

        List<LeaderboardItemResponse> result = new ArrayList<>();
        int rank = 1;

        for (GameRecord record : sortedBestRecords) {
            result.add(new LeaderboardItemResponse(
                    rank++,
                    record.getUser().getUsername(),
                    record.getPoints()
            ));
        }

        return result;
    }
}