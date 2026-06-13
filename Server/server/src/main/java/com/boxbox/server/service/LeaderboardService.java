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

    // Get the overall leaderboard
    public List<LeaderboardItemResponse> getLeaderboard() {
        List<GameRecord> allRecords = gameRecordRepository.findAll();
        return buildLeaderboard(allRecords);
    }

    // Get the leaderboard for a specific stage.
    public List<LeaderboardItemResponse> getStageLeaderboard(Integer stageId) {
        List<GameRecord> stageRecords = gameRecordRepository.findByStageId(stageId);
        return buildLeaderboard(stageRecords);
    }

    // Get the best score for a specific user.
    public BestScoreResponse getUserBestScore(Long userId) {
        User user = userRepository.findById(userId)
                .orElseThrow(() -> new IllegalArgumentException("User not found."));

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

    // Build leaderboard rows using each user's best record.
    private List<LeaderboardItemResponse> buildLeaderboard(List<GameRecord> records) {
        // key: userId, value: the user's best record
        Map<Long, GameRecord> bestRecordByUser = new LinkedHashMap<>();

        for (GameRecord record : records) {
            Long userId = record.getUser().getUserId();

            // Save the first record for this user.
            if (!bestRecordByUser.containsKey(userId)) {
                bestRecordByUser.put(userId, record);
                continue;
            }

            // Compare with the saved best record.
            GameRecord currentBest = bestRecordByUser.get(userId);
            
            // Replace it if the score is higher, or if the same score was achieved earlier.
            if (record.getPoints() > currentBest.getPoints()) {
                bestRecordByUser.put(userId, record);
            } else if (record.getPoints() == currentBest.getPoints()
                    && record.getAchievedAt().isBefore(currentBest.getAchievedAt())) {
                bestRecordByUser.put(userId, record);
            }
        }

        List<GameRecord> sortedBestRecords = new ArrayList<>(bestRecordByUser.values());
        sortedBestRecords.sort(Comparator.comparing(GameRecord::getPoints).reversed()
                                .thenComparing(GameRecord::getAchievedAt));

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