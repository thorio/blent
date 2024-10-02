use std::{env, path::PathBuf};

fn home() -> PathBuf {
	let home = env::var("HOME").expect("$HOME should always be set");

	PathBuf::from(home)
}

pub fn default_apps() -> PathBuf {
	let mut path = home();

	path.push("app");

	path
}
