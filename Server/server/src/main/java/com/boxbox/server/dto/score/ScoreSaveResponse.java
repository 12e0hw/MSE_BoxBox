package com.boxbox.server.dto.score;

import lombok.AllArgsConstructor;
import lombok.Getter;

@Getter
@AllArgsConstructor
// Response DTO returned after saving a player's score.
public class ScoreSaveResponse {

    private Long recordId;
    private Long userId;
    private Integer stageId;
    private Integer points;
}