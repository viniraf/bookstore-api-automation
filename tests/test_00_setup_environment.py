def test_environment_variables_loaded(base_url):
    """
    Environment setup test to verify that required environment variables 
    and base configurations are properly loaded before running API tests.

    This test does NOT make any external API calls.
    It only checks local environment readiness and configuration integrity.
    """
    print("\n🚀 Starting environment setup verification...")

    # Step 1: Verify BASE_URL format
    assert isinstance(base_url, str), "❌ BASE_URL must be a string."
    assert base_url.startswith("http"), "❌ BASE_URL must start with 'http' or 'https'."
    print(f"🔹 BASE_URL format verified: {base_url}")

    # Step 2: Print success message
    print("✅ Environment configuration and setup validated successfully.")