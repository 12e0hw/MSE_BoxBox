package com.boxbox.server.dto;

import lombok.Getter;

@Getter
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