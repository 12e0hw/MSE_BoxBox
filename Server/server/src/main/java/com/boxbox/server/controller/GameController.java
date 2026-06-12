package com.boxbox.server.controller;

import com.boxbox.server.dto.BestScoreResponse;
import com.boxbox.server.dto.LeaderboardItemResponse;
import com.boxbox.server.dto.score.ScoreSaveRequest;
import com.boxbox.server.dto.score.ScoreSaveResponse;
import com.boxbox.server.global.ApiResponse;
import com.boxbox.server.service.GameScoreService;
import com.boxbox.server.service.LeaderboardService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import java.util.List;

@RestController
@RequestMapping("/api/game")
@RequiredArgsConstructor
@CrossOrigin(origins = "*")
public class GameController {

    private final LeaderboardService leaderboardService;
    private final GameScoreService gameScoreService;

    // Save a user's score for the selected stage.
    @PostMapping("/score")
    public ResponseEntity<ApiResponse<ScoreSaveResponse>> saveScore(@RequestBody ScoreSaveRequest request) {
        try {
            ScoreSaveResponse response = gameScoreService.saveScore(request);
            return ResponseEntity.status(201)
                    .body(ApiResponse.success("Score saved successfully.", response));
        } catch (IllegalArgumentException e) {
            return ResponseEntity.status(404)
                    .body(ApiResponse.fail(e.getMessage()));
        }
    }

    // Return the overall leaderboard.
    @GetMapping("/rank")
    public ResponseEntity<ApiResponse<List<LeaderboardItemResponse>>> getLeaderboard() {
        List<LeaderboardItemResponse> response = leaderboardService.getLeaderboard();
        return ResponseEntity.ok(
                ApiResponse.success("Leaderboard loaded successfully.", response)
        );
    }

    // Return the leaderboard for one stage.
    @GetMapping("/rank/{stageId}")
    public ResponseEntity<ApiResponse<List<LeaderboardItemResponse>>> getStageLeaderboard(@PathVariable Integer stageId) {
        List<LeaderboardItemResponse> response = leaderboardService.getStageLeaderboard(stageId);
        return ResponseEntity.ok(
                ApiResponse.success("Stage leaderboard loaded successfully.", response)
        );
    }

    // Return a user's best score.
    @GetMapping("/rank/user/{userId}")
    public ResponseEntity<ApiResponse<BestScoreResponse>> getUserBestScore(@PathVariable Long userId) {
        try {
            BestScoreResponse response = leaderboardService.getUserBestScore(userId);
            return ResponseEntity.ok(
                    ApiResponse.success("Best score loaded successfully.", response)
            );
        } catch (IllegalArgumentException e) {
            // Return 404 when the requested user does not exist.
            return ResponseEntity.status(404)
                    .body(ApiResponse.fail(e.getMessage()));
        }
    }
}
