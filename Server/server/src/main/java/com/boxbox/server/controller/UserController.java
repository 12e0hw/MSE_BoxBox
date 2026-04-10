package com.boxbox.server.controller;

import com.boxbox.server.entity.User;
import com.boxbox.server.service.UserService;
import com.boxbox.server.dto.*;
import com.boxbox.server.global.ApiResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/users")
@RequiredArgsConstructor
@CrossOrigin(origins = "*") 
public class UserController {

    private final UserService userService;

    @PostMapping("/signup")
    public ResponseEntity<ApiResponse<String>> signup(@RequestBody SignupRequest dto) {
        try {
            userService.signup(dto);
            return ResponseEntity.ok(ApiResponse.success("Signup successful", "Success"));
        } catch (IllegalArgumentException e) {
            return ResponseEntity.status(400).body(ApiResponse.fail(e.getMessage()));
        }
    }

    @PostMapping("/login")
    public ResponseEntity<ApiResponse<Long>> login(@RequestBody LoginRequest dto) {
        try {
            User user = userService.login(dto);
            // 로그인 성공 시 유저의 고유 ID(PK)를 넘겨주면 유니티에서 점수 저장할 때 쓰기 좋아요
            return ResponseEntity.ok(ApiResponse.success("Login successful", user.getUserId()));
        } catch (IllegalArgumentException e) {
            return ResponseEntity.status(401).body(ApiResponse.fail(e.getMessage()));
        }
    }
}