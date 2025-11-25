import os
import pytest
import requests
from dotenv import load_dotenv
from src.api.account import generate_token

@pytest.fixture(scope="session")
def auth_token():
    load_dotenv()
    base_url_account = os.getenv("BASE_URL_ACCOUNT")
    user = os.getenv("BOOKSTORE_USERNAME")
    password = os.getenv("BOOKSTORE_PASSWORD")
    if not (base_url_account and user and password):
        pytest.skip("Account base URL or credentials missing; skipping auth fixtures.")
    return generate_token(base_url_account, user, password)

@pytest.fixture(scope="session")
def api_session(auth_token):
    load_dotenv()
    base_url_bookstore = os.getenv("BASE_URL_BOOKSTORE")
    if not base_url_bookstore:
        pytest.skip("Bookstore base URL missing; skipping api_session fixture.")
    session = requests.Session()
    session.headers.update({"Content-Type": "application/json", "Authorization": f"Bearer {auth_token}"})
    yield session, base_url_bookstore
    session.close()