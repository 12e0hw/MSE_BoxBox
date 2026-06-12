package com.boxbox.server.dto;

import lombok.Getter;

@Getter
public class LoginRequest {
    // Login credentials sent from the client.
    private String username;
    private String password;
}
