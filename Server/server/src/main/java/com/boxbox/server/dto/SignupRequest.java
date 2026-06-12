package com.boxbox.server.dto;

import lombok.Getter;

@Getter
public class SignupRequest {
    // Account data required for signup.
    private String username;
    private String password;
}
