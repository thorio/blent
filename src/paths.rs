use crate::ext::Tap;
use std::{env, path::PathBuf};

fn home() -> PathBuf {
	let home = env::var("HOME").expect("$HOME should always be set");

	PathBuf::from(home)
}

pub fn app() -> PathBuf {
	let env = env::var("BLENT_APP_PATH").ok().map(PathBuf::from);

	env.unwrap_or_else(default_app)
}

fn default_app() -> PathBuf {
	home().tap(|p| p.push("app"))
}
