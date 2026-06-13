package com.boxbox.server.dto.score;

import lombok.Getter;
import lombok.NoArgsConstructor;

@Getter
@NoArgsConstructor
// Request DTO for saving a player's score.
public class ScoreSaveRequest {

    private Long userId;
    private Integer stageId;
    private Integer points;
}