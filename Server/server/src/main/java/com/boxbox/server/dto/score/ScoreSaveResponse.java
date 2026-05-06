package com.boxbox.server.dto.score;

import lombok.AllArgsConstructor;
import lombok.Getter;

@Getter
@AllArgsConstructor
public class ScoreSaveResponse {

    private Long recordId;
    private Long userId;
    private Integer stageId;
    private Integer points;
}