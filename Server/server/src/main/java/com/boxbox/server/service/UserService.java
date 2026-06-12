package com.boxbox.server.service;

import com.boxbox.server.entity.User;
import com.boxbox.server.repository.UserRepository;
import com.boxbox.server.dto.*;

import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
@RequiredArgsConstructor
public class UserService {

    private final UserRepository userRepository;

    // Create a new account after checking duplicate usernames.
    public void signup(SignupRequest dto) {
        if (userRepository.existsByUsername(dto.getUsername())) {
            throw new IllegalArgumentException("Username already exists.");
        }

        User user = new User(dto.getPassword(), dto.getUsername());
        userRepository.save(user);
    }

    // Validate login credentials and return the matched user.
    public User login(LoginRequest dto) {
        User user = userRepository.findByUsername(dto.getUsername())
                .orElseThrow(() -> new IllegalArgumentException("Username does not exist."));

        if (!user.getPassword().equals(dto.getPassword())) {
            throw new IllegalArgumentException("Password does not match.");
        }
        return user;
    }

    @Transactional
    public void saveCharacterName(NameSave request) {
        // Update one of the two saved character name slots.
        User user = userRepository.findById((long) request.getUserId())
            .orElseThrow(() -> new IllegalArgumentException("User not found."));

        if (request.getIndex() == 1) {
            user.updateCharacterName1(request.getCharacterName());
        } else if (request.getIndex() == 2) {
            user.updateCharacterName2(request.getCharacterName());
        } else {
            throw new IllegalArgumentException("Index must be 1 or 2.");
        }
    }

    public NameSave loadCharacterName(int userId, int index) {
        // Read one of the two saved character name slots.
        User user = userRepository.findById((long) userId)
            .orElseThrow(() -> new IllegalArgumentException("User not found."));

        String loadedName = "";

        if (index == 1) {
            loadedName = user.getCharacterName1();
        } else if (index == 2) {
            loadedName = user.getCharacterName2();
        } else {
            throw new IllegalArgumentException("Index must be 1 or 2.");
        }

        NameSave responseDto = new NameSave();
        responseDto.setUserId(userId);
        responseDto.setIndex(index);
        responseDto.setCharacterName(loadedName);

        return responseDto;
    }
}
