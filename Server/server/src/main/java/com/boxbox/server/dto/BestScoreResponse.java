package com.boxbox.server.dto;

import lombok.Getter;

@Getter
// Response DTO for a user's best score.
public class BestScoreResponse {

    private final Long userId;
    private final String username;
    private final int bestScore;

    public BestScoreResponse(Long userId, String username, int bestScore) {
        this.userId = userId;
        this.username = username;
        this.bestScore = bestScore;
    }
}