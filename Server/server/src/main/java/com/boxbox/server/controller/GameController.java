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

    // 점수 저장
    @PostMapping("/score")
    public ResponseEntity<ApiResponse<ScoreSaveResponse>> saveScore(@RequestBody ScoreSaveRequest request) {
        try {
            ScoreSaveResponse response = gameScoreService.saveScore(request);
            return ResponseEntity.status(201)
                    .body(ApiResponse.success("점수 저장 성공", response));
        } catch (IllegalArgumentException e) {
            return ResponseEntity.status(404)
                    .body(ApiResponse.fail(e.getMessage()));
        }
    }

    // 전체 리더보드 조회 
    @GetMapping("/rank")
    public ResponseEntity<ApiResponse<List<LeaderboardItemResponse>>> getLeaderboard() {
        List<LeaderboardItemResponse> response = leaderboardService.getLeaderboard();
        // 성공 메시지와 함께 JSON 형태로 반환
        return ResponseEntity.ok(
                ApiResponse.success("리더보드 조회 성공", response)
        );
    }

    // 스테이지별 리더보드 조회 
    @GetMapping("/rank/{stageId}")
    public ResponseEntity<ApiResponse<List<LeaderboardItemResponse>>> getStageLeaderboard(@PathVariable Integer stageId) {
        List<LeaderboardItemResponse> response = leaderboardService.getStageLeaderboard(stageId);
        return ResponseEntity.ok(
                ApiResponse.success("스테이지 리더보드 조회 성공", response)
        );
    }

    // 내 최고 점수 조회
    @GetMapping("/rank/user/{userId}")
    public ResponseEntity<ApiResponse<BestScoreResponse>> getUserBestScore(@PathVariable Long userId) {
        try {
            BestScoreResponse response = leaderboardService.getUserBestScore(userId);
            return ResponseEntity.ok(
                    ApiResponse.success("최고 점수 조회 성공", response)
            );
        } catch (IllegalArgumentException e) {
            // 유저가 존재하지 않으면 404 응답 반환
            return ResponseEntity.status(404)
                    .body(ApiResponse.fail(e.getMessage()));
        }
    }
}