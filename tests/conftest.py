import os
import pytest
from dotenv import load_dotenv

@pytest.fixture(scope="session")
def base_url():
    """
    Fixture responsible for loading and returning the BASE_URL from environment variables.

    Fails immediately if BASE_URL is missing from the .env file.
    This ensures the API base endpoint is properly configured before running any tests.
    """
    load_dotenv()
    base_url = os.getenv("BASE_URL")

    if not base_url:
        raise ValueError(
            "❌ BASE_URL not found in environment variables.\n"
            "Please check your .env file and ensure it contains a valid BASE_URL entry."
        )

    print(f"✅ BASE_URL loaded successfully: {base_url}")
    return base_url