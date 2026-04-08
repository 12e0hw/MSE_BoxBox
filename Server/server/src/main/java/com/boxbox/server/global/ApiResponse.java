package com.boxbox.server.global;

public class ApiResponse<T> {

    private boolean success;
    private String message;
    // 실제 데이터
    // T는 "나중에 타입이 정해지는 자리"라고 생각하면 됨
    // 예: String, UserDto, List<LeaderboardItemResponse> 등
    private T data;

    public ApiResponse() {
    }

    public ApiResponse(boolean success, String message, T data) {
        this.success = success;
        this.message = message;
        this.data = data;
    }

    // 성공 응답 메서드
    // 예: ApiResponse.success("조회 성공", user)
    public static <T> ApiResponse<T> success(String message, T data) {
        return new ApiResponse<>(true, message, data);
    }

    // 실패 응답 메서드
    // 실패할 때는 보통 data가 없으므로 null로 넣음
    // 예: ApiResponse.fail("존재하지 않는 사용자입니다.")
    public static <T> ApiResponse<T> fail(String message) {
        return new ApiResponse<>(false, message, null);
    }

    public boolean isSuccess() {
        return success;
    }

    public String getMessage() {
        return message;
    }

    public T getData() {
        return data;
    }
}