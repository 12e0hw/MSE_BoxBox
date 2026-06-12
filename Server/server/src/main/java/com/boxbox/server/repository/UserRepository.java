package com.boxbox.server.repository;

import com.boxbox.server.entity.User; 
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import java.util.Optional;

@Repository
public interface UserRepository extends JpaRepository<User, Long> {
    
    // Used to block duplicate usernames during signup.
    boolean existsByUsername(String username);

    // Used for login and account lookup by username.
    Optional<User> findByUsername(String username);
}
