import os
import pytest
from dotenv import load_dotenv
import requests

@pytest.fixture(scope="session")
def auth_token():
        """
        Obtain a session-scoped Bearer token from the BookStore API.

        This fixture loads credentials from the environment (via python-dotenv)
        and performs a single request to the GenerateToken endpoint. It
        executes once per pytest session and returns the token string.

        Returns:
            str: Bearer token.

        Raises:
            ValueError: If required environment variables are missing.
            requests.HTTPError: If the token request returns an HTTP error.
            RuntimeError: If the response payload does not contain a token.
        """

        load_dotenv()
        user = os.getenv("BOOKSTORE_USERNAME")
        password = os.getenv("BOOKSTORE_PASSWORD")

        print(f"\nUsing USERNAME: {user}")
        print(f"Using PASSWORD: {password}")

        if not user or not password:
                raise ValueError(
                        "USERNAME and PASSWORD must be set in environment or .env to obtain auth token."
                )

        url = os.getenv("BASE_URL_ACCOUNT") + "/GenerateToken"
        payload = {"userName": user, "password": password}

        resp = requests.post(url, json=payload, timeout=10)
        resp.raise_for_status()

        data = resp.json()
        token = data.get("token")
        if not token:
                raise RuntimeError(f"Token not found in response: {data}")

        print("\n✅ Auth token obtained successfully.")
        print(f"🔑 Token: {token}")
        return token