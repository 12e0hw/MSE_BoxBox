package com.boxbox.server.dto.score;

import lombok.Getter;
import lombok.NoArgsConstructor;

@Getter
@NoArgsConstructor
public class ScoreSaveRequest {

    private Long userId;
    private Integer stageId;
    private Integer points;
}