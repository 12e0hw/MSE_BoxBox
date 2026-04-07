package com.boxbox.server.controller;

import com.boxbox.server.entity.GameRecord;
import com.boxbox.server.entity.User;
import com.boxbox.server.repository.GameRecordRepository;
import com.boxbox.server.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.*;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/api/game")
@CrossOrigin(origins = "*")
public class GameController {

    @Autowired
    private GameRecordRepository gameRecordRepository;
    
    @Autowired
    private UserRepository userRepository;

    // 전체 리더보드 조회 
    @GetMapping("/rank")
    public List<Map<String, Object>> getGlobalRank() {
        List<GameRecord> records = gameRecordRepository.findAllByOrderByPointsDesc();
        return convertToRankList(records);
    }

    // 스테이지별 리더보드 조회 
    @GetMapping("/rank/{stageId}")
    public List<Map<String, Object>> getStageRank(@PathVariable int stageId) {
        List<GameRecord> records = gameRecordRepository.findByStageIdOrderByPointsDesc(stageId);
        return convertToRankList(records);
    }

    // 내 최고 점수 조회 (GET /api/game/rank/user/{userId})
    // @GetMapping("/rank/user/{userId}")
    // public Map<String, Object> getUserBestScore(@PathVariable Long userId) {
    //     User user = userRepository.findById(userId).orElseThrow(() -> new RuntimeException("User not found"));
    //     GameRecord best = gameRecordRepository.findFirstByUserOrderByPointsDesc(user)
    //             .orElse(null);

    //     Map<String, Object> response = new HashMap<>();
    //     response.put("userId", userId);
    //     response.put("username", user.getUsername());
    //     response.put("bestScore", (best != null) ? best.getPoints() : 0);
    //     return response;
    // }

    // DB 데이터를 명세서의 [rank, username, score] 형식으로 변환
    private List<Map<String, Object>> convertToRankList(List<GameRecord> records) {
        List<Map<String, Object>> rankList = new ArrayList<>();
        for (int i = 0; i < records.size(); i++) {
            GameRecord r = records.get(i);
            Map<String, Object> map = new HashMap<>();
            map.put("rank", i + 1); // 리스트 순서대로 순위
            map.put("username", r.getUser().getUsername());
            map.put("score", r.getPoints());
            rankList.add(map);
        }
        return rankList;
    }
}