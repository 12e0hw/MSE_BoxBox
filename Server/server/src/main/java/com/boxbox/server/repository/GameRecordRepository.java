package com.boxbox.server.repository;

import com.boxbox.server.entity.GameRecord;
// import com.boxbox.server.entity.User;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;
// import java.util.Optional;

public interface GameRecordRepository extends JpaRepository<GameRecord, Long> {
    
    // 리더보드 테스트용
    List<GameRecord> findByUserUserId(Long userId);

    List<GameRecord> findByStageId(Integer stageId);

    List<GameRecord> findByUserUserIdAndStageId(Long userId, Integer stageId);

    // 점수 높은 순으로 전체 조회
    List<GameRecord> findAllByOrderByPointsDesc();

    // 특정 스테이지 필터링 후 점수순 정렬
    List<GameRecord> findByStageIdOrderByPointsDesc(int stageId);

    // 내 최고 점수 조회: 특정 유저의 점수 중 가장 높은 것 하나만 가져옴
    // Optional<GameRecord> findFirstByUserOrderByPointsDesc(User user);
}