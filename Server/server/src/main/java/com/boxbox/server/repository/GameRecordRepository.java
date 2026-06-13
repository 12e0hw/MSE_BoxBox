package com.boxbox.server.repository;

import com.boxbox.server.entity.GameRecord;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface GameRecordRepository extends JpaRepository<GameRecord, Long> {
    
    // Find records by user ID.
    List<GameRecord> findByUserUserId(Long userId);

    List<GameRecord> findByStageId(Integer stageId);

    List<GameRecord> findByUserUserIdAndStageId(Long userId, Integer stageId);

    // Find all records ordered by score descending.
    List<GameRecord> findAllByOrderByPointsDesc();

    // Find records for a specific stage ordered by score descending.
    List<GameRecord> findByStageIdOrderByPointsDesc(int stageId);
}