package com.boxbox.server.service;

import com.boxbox.server.entity.User;
import com.boxbox.server.repository.UserRepository;
import com.boxbox.server.dto.*;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class UserService {

    private final UserRepository userRepository;

    // Signup
    public void signup(SignupRequest dto) {
        if (userRepository.existsByUsername(dto.getUsername())) {
            throw new IllegalArgumentException("Duplicate Username");
        }

        User user = new User(dto.getPassword(), dto.getUsername());
        userRepository.save(user);
    }

    // Login
    public User login(LoginRequest dto) {
        User user = userRepository.findByUsername(dto.getUsername())
                .orElseThrow(() -> new IllegalArgumentException("Username does not exist"));

        if (!user.getPassword().equals(dto.getPassword())) {
            throw new IllegalArgumentException("Password Mismatch");
        }
        return user;
    }
}