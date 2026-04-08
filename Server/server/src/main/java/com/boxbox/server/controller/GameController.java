package com.boxbox.server.controller;

import com.boxbox.server.dto.BestScoreResponse;
import com.boxbox.server.dto.LeaderboardItemResponse;
import com.boxbox.server.entity.GameRecord;
import com.boxbox.server.entity.User;
import com.boxbox.server.global.ApiResponse;
import com.boxbox.server.service.LeaderboardService;

import lombok.RequiredArgsConstructor;

import com.boxbox.server.repository.GameRecordRepository;
import com.boxbox.server.repository.UserRepository;
import com.boxbox.server.service.LeaderboardService;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.*;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/api/game")
@RequiredArgsConstructor
@CrossOrigin(origins = "*")
public class GameController {

    private final LeaderboardService leaderboardService;

    @Autowired
    private GameRecordRepository gameRecordRepository;
    
    @Autowired
    private UserRepository userRepository;

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

    // 내 최고 점수 조회 (GET /api/game/rank/user/{userId})
    @GetMapping("rank/user/{userId}")
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