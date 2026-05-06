package com.boxbox.server.service;

import com.boxbox.server.dto.score.ScoreSaveRequest;
import com.boxbox.server.dto.score.ScoreSaveResponse;
import com.boxbox.server.entity.GameRecord;
import com.boxbox.server.entity.User;
import com.boxbox.server.repository.GameRecordRepository;
import com.boxbox.server.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
@RequiredArgsConstructor
public class GameScoreService {

    private final GameRecordRepository gameRecordRepository;
    private final UserRepository userRepository;

    @Transactional
    public ScoreSaveResponse saveScore(ScoreSaveRequest request) {
        User user = userRepository.findById(request.getUserId())
                .orElseThrow(() -> new IllegalArgumentException("존재하지 않는 유저입니다."));

        GameRecord gameRecord = new GameRecord();
        gameRecord.setUser(user);
        gameRecord.setStageId(request.getStageId());
        gameRecord.setPoints(request.getPoints());

        GameRecord savedRecord = gameRecordRepository.save(gameRecord);

        return new ScoreSaveResponse(
                savedRecord.getRecordId(),
                savedRecord.getUser().getUserId(),
                savedRecord.getStageId(),
                savedRecord.getPoints()
        );
    }
}